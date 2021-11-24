using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Networking.Client;
using UnityEngine;

namespace Networking.NetworkTest
{
    public class MessageSender : MonoBehaviour
    {

        [SerializeField] private string message;

        [SerializeField] private DataSender sender;
        private BinaryFormatter _converter;


        private void Start()
        {
            _converter = new BinaryFormatter();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
