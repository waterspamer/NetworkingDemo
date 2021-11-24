using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Networking.JsonUtils
{
    public static class JsonDataWrapper
    {
        private static JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        
        public static string SerializeData(List<SerializableData> objectsToSerialize) =>
            JsonConvert.SerializeObject(objectsToSerialize, _settings);
        
        public static List<SerializableData> DeserializeData(string dataBuffer) =>
            JsonConvert.DeserializeObject<List<SerializableData>>(dataBuffer, _settings);


        private static void Copy(object parent, JObject child)
        {
            var parentProperties = parent.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in parentProperties)
            {
                var tst = child[propertyInfo.Name];
                var type = tst.ToObject(propertyInfo.FieldType);
                propertyInfo.SetValue(parent, type);
            }
        }
        
        public static void ApplyReceivedDataToGameObjects(SerializableData serializableData)
        {
            var copiedObjectInstance = Activator.CreateInstance(serializableData.PropertyType);
            var gameObject = GameObject.Find(serializableData.GameObjectName);
            foreach (var property in serializableData.FieldsValues)
            {
                var settedPropertyValue = property.Item2;
                Copy(copiedObjectInstance, settedPropertyValue as JObject);
                var obj = gameObject.GetComponent(serializableData.ComponentType);
                obj.GetType().GetProperty(property.Item1)?.SetValue( obj, copiedObjectInstance);
            }
        }            
    }
}