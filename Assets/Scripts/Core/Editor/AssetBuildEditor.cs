using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LeyoutechEditor.Core.Packer;
public class AssetBuildEditor : Editor
{
    [MenuItem("AssetBuild/ToBuild", false)]
    static public void ToBuild()
    {
        Debug.Log("ToBuild");
    }
    [MenuItem("AssetBuild/SetName", false)]
    static public void SetName()
    {
        Debug.Log("SetName");
        if (!Application.isPlaying)

        {
            bool result = BundlePackUtil.GenerateConfigs(true, true, true);
            if (!result)
            {
                if (EditorUtility.DisplayDialog("Warning", "Address Repeat!!!!\nDo you want to fix or view??", "OK", "Cancel"))
                {
                    BundlePackWindow.ShowWin();
                }
            }
            else
            {
                if (EditorUtility.DisplayDialog("Success", "Packed Success", "OK"))
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
