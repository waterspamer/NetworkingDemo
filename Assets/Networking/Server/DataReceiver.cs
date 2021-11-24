using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Networking.JsonUtils;
using UnityEngine;

namespace Networking.Server
{
    public class DataReceiver : MonoBehaviour
    {
        private string _dataBuffer;

        // Start is called before the first frame update
        void Start()
        {
            _dataBuffer = string.Empty;
            StartReceiving();
        }
        private IPEndPoint _remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private  CancellationTokenSource _cToken ;

        public void StartReceiving()
        {

            UdpClient _receivingUdpClient = new UdpClient(11000);
            Task.Run(() =>
            {
                while (!_cToken.IsCancellationRequested)
                {
                    try
                    {
                        var receiveBytes = _receivingUdpClient.Receive(ref _remoteIpEndPoint);
                        _dataBuffer = Encoding.ASCII.GetString(receiveBytes);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }
            });
           
        }

        

        private void OnApplicationQuit()
        {
            _cToken.Cancel();
        }

        // Update is called once per frame
        void Update()
        {
            if (!String.IsNullOrEmpty(_dataBuffer))
            {
                Debug.Log(JsonDataWrapper.DeserializeData(_dataBuffer).Last().FieldsValues.Last().Item1);
                var dataList = JsonDataWrapper.DeserializeData(_dataBuffer);
                foreach (var data in dataList)
                {
                    JsonDataWrapper.ApplyReceivedDataToGameObjects(data);
                }
            }
        }
    }
}