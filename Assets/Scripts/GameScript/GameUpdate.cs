using System.Collections;
using System.Collections.Generic;
using Leyoutech.Core.Loader;
using UnityEngine;
public class GameUpdate : MonoSingleton<GameUpdate>
{


    void Update()
    {
        AssetManager.GetInstance().DoUpdate(Time.deltaTime);
    }
}