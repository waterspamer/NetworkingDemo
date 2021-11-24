using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectiveTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var pInfo = GameObject.Find("Cube").GetComponent(typeof(Transform)).GetType().GetProperty("position");
        Debug.Log(pInfo);
        var obj = GameObject.Find("Cube").GetComponent(typeof(Transform));
        GameObject.Find("Cube").GetComponent(typeof(Transform)).GetType().GetProperty("position").SetValue( obj, Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
