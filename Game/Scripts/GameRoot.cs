using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Yifan.Core;

public class GameRoot : MonoBehaviour
{
    private Simulator simulator = new Simulator();
    public static GameRoot Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);

        AssetManager.Instance.CreateGameObject(new AssetId("actors/role/1001001", "1001001"), null);
    }
}

