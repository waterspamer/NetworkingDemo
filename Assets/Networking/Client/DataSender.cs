using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Networking.JsonUtils;
using Newtonsoft.Json;
using UnityEngine;

namespace Networking.Client
{
    public class DataSender : MonoBehaviour
    {
        private UdpClient _client = new UdpClient();

        private JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    
        IPEndPoint ipEndPint = new IPEndPoint(IPAddress.Broadcast, 11000);

        private void Sender(List<SerializableData> serializableData)
        {
            var serialized = JsonDataWrapper.SerializeData(serializableData);
            Byte[] sendBytes = Encoding.ASCII.GetBytes(serialized);
            
            try{
                _client.Send(sendBytes, sendBytes.Length, ipEndPint);
            }
            catch ( Exception e ){
                Debug.Log(e.ToString());	
            }
        }
    

        private void Update()
        {
            var data = new List<SerializableData>
            {
                new SerializableData(
                    gameObject.name, typeof(Transform), gameObject.GetComponent<Transform>().GetType().GetProperty("position"),
                    gameObject.transform.position),
                new SerializableData(
                    gameObject.name, typeof(Transform), gameObject.GetComponent<Transform>().GetType().GetProperty("localScale"),
                    gameObject.transform.localScale),
                new SerializableData(
                    gameObject.name, typeof(Transform), gameObject.GetComponent<Transform>().GetType().GetProperty("rotation"),
                    gameObject.transform.rotation),
            };
            Sender(data);
        }
    }
}
