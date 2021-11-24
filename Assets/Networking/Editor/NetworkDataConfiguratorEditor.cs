using System;
using System.Collections.Generic;
using Networking.Client;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Networking.Editor
{
    [CustomEditor(typeof(NetworkDataConfigurator))]
    public class NetworkDataConfiguratorEditor : UnityEditor.Editor
    {
        SerializedProperty goprop;
        private GameObject _gameObject;
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            
            goprop.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("GameObject", goprop.objectReferenceValue, typeof(GameObject), true);
            serializedObject.ApplyModifiedProperties();
            List<string> options = new List<string>();
            int selected = 0;
            foreach (var component in ((GameObject)goprop.objectReferenceValue).GetComponents<Component>())
                {
                    options.Add(component.name);
                    
                }
            selected = EditorGUILayout.Popup("Components", selected, options.ToArray()); 
            
            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            goprop = serializedObject.FindProperty("serializedGameObject");
        }
    }
}
