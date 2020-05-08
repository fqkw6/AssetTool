using System.Collections;
using System.Collections.Generic;
using Leyoutech.Core.Loader;
using UnityEngine;
using Leyoutech.Core.Loader.Config;
public class GameLanch : MonoBehaviour
{

    void Start()
    {
        GameUpdate.Instance.Init();
        //AssetTool.Instance.Init();
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
                    Debug.LogError("sssdsdsd");
                    Leyoutech.Core.Loader.AssetManager.GetInstance().LoadAssetAsync(AssetAddressKey.PREFABS_CUBE, (address, uObj, userData) =>
                                {
                                    GameObject go = uObj as GameObject;
                                    Debug.LogError(go);
                                });
                }
            });

    }

    void Update()
    {

    }
}