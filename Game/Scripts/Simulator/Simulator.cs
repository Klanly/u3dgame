using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yifan.Core;

class Simulator
{
    public void StartGame()
    {
        this.LoadScene();
    }

    private void LoadScene()
    {
        AssetManager.Instance.CreateScene(
            new AssetId("scene/1", "Scene1"),
            LoadSceneMode.Single,
            (AssetId asset_id) =>
            {
                this.CreateRole();
            });
    }

    private void CreateRole()
    {
        AssetManager.Instance.CreateGameObject(
            new AssetId("actors/role/1001001", "1001001"),
            (GameObject role) =>
            {
                role.transform.position = new Vector3(-99.6f, 312.4f, 66.7f);
                CameraFollow camer_follow = Camera.main.GetComponent<CameraFollow>();
                camer_follow.Target = role.transform;
            });
    }
}
