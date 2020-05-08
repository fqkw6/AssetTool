using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;
using Leyoutech.Core.Loader;
public class AssetTool : MonoSingleton<AssetTool>
{
    private AssetLoaderState m_State;

    private float m_Progress = 0f;

    private AssetBundleCreateRequest m_asyncOperation = null;
    //本地加载
    private UnityObject LoadAssetFromLocal(string path, Action<UnityObject> callback)
    {
        return AssetDatabase.LoadAssetAtPath(path, typeof(UnityObject));
    }

    private AssetBundleCreateRequest LoadAssetFromBuild(string path, Action<UnityObject> callback)
    {
        return AssetBundle.LoadFromFileAsync(path);
    }

    public void LoadAsset(string path, Action<UnityObject> callback)
    {
        m_State = AssetLoaderState.Loading;
        if (GameConst.m_AssetLoaderMode == AssetLoaderMode.AssetDatabase)
        {
            UnityObject obj = LoadAssetFromLocal(path, callback);
            m_State = AssetLoaderState.Complete;
            m_Progress = 1;
            callback(obj);
        }
        else
        {
            m_asyncOperation = LoadAssetFromBuild(path, callback);
            m_State = AssetLoaderState.Complete;
            m_Progress = m_asyncOperation.progress;

        }
    }
}
