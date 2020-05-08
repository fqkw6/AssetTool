using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HelpTest : MonoBehaviour {
    private static string GetName () {
        var type = typeof (CustomAttributes);
        var field = type.GetField ("Address");
        var v = field.GetCustomAttributes (typeof (TestNameAttribute), false);
        Debug.LogError ((v[0] as TestNameAttribute).Name);
        var attribute = type.GetCustomAttributes (typeof (TestNameAttribute), false);
        if (attribute == null) {
            return null;
        }

        return (attribute[0] as TestNameAttribute).Name;

    }
    private void Start () {
        GetName ();
    }
}