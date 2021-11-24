using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Networking.JsonUtils
{
    [System.Serializable]
    public class SerializableData
    {
        public string GameObjectName;
        public Type ComponentType;
        public Type PropertyType;
        
        public List<Tuple<string, object>> FieldsValues;

        public SerializableData()
        {
        }

        public SerializableData(string gameObjectName, Type componentType, PropertyInfo property, object value)
        {
            PropertyType = property.PropertyType;
            GameObjectName = gameObjectName;
            ComponentType = componentType;
            FieldsValues = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>(property.Name, value)
            };
        }

        public SerializableData(string gameObjectName, Type componentType, List<Tuple<string, object>> fieldsValues)
        {
            GameObjectName = gameObjectName;
            ComponentType = componentType;
            FieldsValues = fieldsValues;
        }

        public override string ToString() =>
            @$"Json data send: {GameObjectName}, component type = {ComponentType}, {string.Join(Environment.NewLine,
                FieldsValues.Select(x => $"Type: {x.Item1} Value:{x.Item2}"))}";
    }
}