using System.Collections;
using System.Collections.Generic;
using Leyoutech.Core.Loader;
using UnityEngine;
public class GameLanch : MonoBehaviour
{

    void Start()
    {
        GameUpdate.Instance.Init();
        AssetTool.Instance.Init();
        AssetBundle.UnloadAllAssetBundles(true);

        AssetManager.GetInstance().InitLoader(
            GameConst.m_AssetLoaderMode,
            AssetPathMode.Address,
            GameConst.m_MaxLoadCount,
            GameConst.m_AssetBundlePath,
            (isSuccess) =>
            {
                if (isSuccess)
                {
                    Debug.LogError("重启");
                }
            });
        AssetTool.Instance.LoadAsset("Assets/AssetPackage/Prefabs/Cube.prefab", (go) =>
        {
            Instantiate(go);
        });
    }

    void Update()
    {

    }
}