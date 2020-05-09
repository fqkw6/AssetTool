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
        Debug.Log(GameConst.GetInstance().m_AssetLoaderMode);
        AssetManager.GetInstance().InitLoader(
            GameConst.GetInstance().m_AssetLoaderMode,
            AssetPathMode.Address,
            GameConst.m_MaxLoadCount,
            GameConst.GetInstance().m_AssetBundlePath,
            (isSuccess) =>
            {
                if (isSuccess)
                {
                    GameLoader.Instance.Init();
                    Debug.Log("重启AssetManager");
                    AssetManager.GetInstance().LoadSceneAsync("Assets/Scenes/GameMain.unity", (address, scene, userData) =>
                         {
                             Debug.Log("加载游戏");
                             AssetManager.GetInstance().LoadAssetAsync(AssetAddressKey.PREFABS_CUBE,
                              (address1, uobj, userData1) =>
                              {
                                  GameObject go = uobj as GameObject;
                                  Debug.Log("chenggong" + go);
                                  GameObject.Instantiate(go);
                              });
                         }, null);
                }
            });

    }


}