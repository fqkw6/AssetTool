using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AssetBuildEditor : Editor
{
   [MenuItem("AssetBuild/ToBuild" , false)]
    static public void ToBuild()
    {
        Debug.Log("ToBuild");
    }
   [MenuItem("AssetBuild/SetName" , false)]
    static public void SetName()
    {
        Debug.Log("SetName");
    }
}
