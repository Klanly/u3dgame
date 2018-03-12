using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yifan.Scene
{
    public class ActorController : MonoBehaviour
    {
        private ActorBlinker actorBlinker;
        private ActorFadeout actorFadeout;

        private void Awake()
        {
            this.actorBlinker = this.GetComponent<ActorBlinker>();
            this.actorFadeout = this.GetComponent<ActorFadeout>();
        }

        public void Blink()
        {
            if (null != this.actorBlinker)
            {
                this.actorBlinker.Blink();
            }
        }

        public void FadeOut(float time, Action callback)
        {
            if (null != this.actorFadeout)
            {
                this.actorFadeout.Fadeout(time, callback);
            }
        }
    }
}
