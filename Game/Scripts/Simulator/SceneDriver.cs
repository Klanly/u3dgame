using HedgehogTeam.EasyTouch;
using System;
using System.Collections.Generic;
using UnityEngine;
using Yifan.Core;
using Yifan.Scene;

class SceneDriver : MonoBehaviour
{
    private GameObject role;

    private void Awake()
    {
        if (GameRoot.Instance != null)
        {
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.AddComponent<EasyTouch>();
            this.CreateRole();
        }
    }

    private void Start()
    {
        SceneInteractive.Instance.ListenClickGround(this.OnClickGround);
    }

    private void CreateRole()
    {
        AssetManager.Instance.CreateGameObject(
            new AssetId("actors/role/1001001", "1001001"),
            (GameObject role) =>
            {
                if (null == role)
                {
                    return;
                }

                this.role = role;
                this.role.transform.position = new Vector3(232, 269, 139);
                CameraFollow camer_follow = Camera.main.GetComponent<CameraFollow>();
                camer_follow.Target = this.role.transform;
                this.role.AddComponent<MoveableObj>();
            });
    }

    private void OnClickGround(Vector3 pos)
    {
        if (null == this.role)
        {
            return;
        }

        MoveableObj move_obj = this.role.GetComponent<MoveableObj>();
        if (null != move_obj)
        {
            move_obj.MoveTo(pos, 3);
        }
    }

 
}
