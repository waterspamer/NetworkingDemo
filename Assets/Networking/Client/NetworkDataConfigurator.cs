using System.Collections.Generic;
using Networking.JsonUtils;
using UnityEngine;

namespace Networking.Client
{
    public class NetworkDataConfigurator : MonoBehaviour
    {
        public GameObject serializedGameObject;
        
        public List<SerializableData> contentToSend;
    }
}
