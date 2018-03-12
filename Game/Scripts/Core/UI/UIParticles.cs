using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yifan.Core
{
    [RequireComponent(typeof(ParticleSystem))]
    public class UIParticles : MaskableGraphic
    {
        public enum UiParticleRenderMode
        {
            Billboard,
            StreachedBillboard
        }

        [SerializeField]
        private UiParticleRenderMode renderMode;

        [SerializeField]
        private bool isSupportClip = false;

        [SerializeField]
        private bool isIgnoreTimescale = false;

        [SerializeField]
        private float stretchedLenghScale = 1f;

        [SerializeField]
        private float stretchedSpeedScale = 1f;

        private ParticleSystem ps;
        private ParticleSystemRenderer ps_render;
        private static ParticleSystem.Particle[] particles;

        public UiParticleRenderMode RenderMode
        {
            get { return renderMode; }
        }

        protected override void Awake()
        {
            base.Awake();

            this.ps = this.GetComponent<ParticleSystem>();
            this.ps_render = this.GetComponent<ParticleSystemRenderer>();
            if (this.m_Material == null)
            {
                this.m_Material = this.ps_render.sharedMaterial;
            }
            if (ps_render.renderMode == ParticleSystemRenderMode.Stretch)
            {
                this.renderMode = UiParticleRenderMode.StreachedBillboard;
            }
            this.SetAllDirty();
        }

        public override void SetMaterialDirty()
        {
            base.SetMaterialDirty();
            if (this.ps_render != null)
            {
                this.ps_render.sharedMaterial = this.m_Material;
            }    
        }

        public override Material GetModifiedMaterial(Material baseMaterial)
        {
            if (this.isSupportClip)
            {
                var mat = base.GetModifiedMaterial(baseMaterial);
                var matClip = new Material(mat);
                matClip.EnableKeyword("ENABLE_UI_CLIP");
                matClip.hideFlags = HideFlags.DontSave;
                return matClip;
            }
            else
            {
                return baseMaterial;
            }
        }

        public override Texture mainTexture
        {
            get
            {
                if (material != null && material.mainTexture != null)
                {
                    return material.mainTexture;
                }
                return s_WhiteTexture;
            }
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (null == this.ps)
            {
                base.OnPopulateMesh(toFill);
                return;
            }

            this.GenerateParticlesBillboards(toFill);
        }

        protected virtual void Update()
        {
            if (!isIgnoreTimescale)
            {
                if (this.ps != null && this.ps.isPlaying)
                {
                    SetVerticesDirty();
                }
            }
            else
            {
                if (this.ps != null)
                {
                    this.ps.Simulate(Time.unscaledDeltaTime, true, false);
                    SetVerticesDirty();
                }
            }

            if (this.ps_render != null && this.ps_render.enabled)
            {
                this.ps_render.enabled = false;
            }
        }

        private void GenerateParticlesBillboards(VertexHelper vh)
        {
            if (null == UIParticles.particles || UIParticles.particles.Length < this.ps.main.maxParticles)
            {
                UIParticles.particles = new ParticleSystem.Particle[this.ps.main.maxParticles];
            }

            vh.Clear();
            int numParticlesAlive = this.ps.GetParticles(UIParticles.particles);
            for (int i = 0; i < numParticlesAlive; i++)
            {
                this.DrawParticleBillboard(UIParticles.particles[i], vh);
            }
        }

        private void DrawParticleBillboard(ParticleSystem.Particle particle, VertexHelper vh)
        {
            var center = particle.position;
            var rotation = Quaternion.Euler(particle.rotation3D);

            if (this.ps.main.simulationSpace == ParticleSystemSimulationSpace.World)
            {
                center = rectTransform.InverseTransformPoint(center);
            }

            float timeAlive = particle.startLifetime - particle.remainingLifetime;
            float globalTimeAlive = timeAlive / particle.startLifetime;

            Vector3 size3D = particle.GetCurrentSize3D(this.ps);

            if (this.renderMode == UiParticleRenderMode.StreachedBillboard)
            {
                GetStrechedBillboardsSizeAndRotation(particle, globalTimeAlive, ref size3D, out rotation);
            }

            var leftTop = new Vector3(-size3D.x * 0.5f, size3D.y * 0.5f);
            var rightTop = new Vector3(size3D.x * 0.5f, size3D.y * 0.5f);
            var rightBottom = new Vector3(size3D.x * 0.5f, -size3D.y * 0.5f);
            var leftBottom = new Vector3(-size3D.x * 0.5f, -size3D.y * 0.5f);


            leftTop = rotation * leftTop + center;
            rightTop = rotation * rightTop + center;
            rightBottom = rotation * rightBottom + center;
            leftBottom = rotation * leftBottom + center;

            Color32 color32 = particle.GetCurrentColor(this.ps);
            var i = vh.currentVertCount;

            Vector2[] uvs = new Vector2[4];

            if (!this.ps.textureSheetAnimation.enabled)
            {
                this.EvaluateQuadUVs(uvs);
            }
            else
            {
                this.EvaluateTexturesheetUVs(particle, timeAlive, uvs);
            }

            vh.AddVert(leftBottom, color32, uvs[0]);
            vh.AddVert(leftTop, color32, uvs[1]);
            vh.AddVert(rightTop, color32, uvs[2]);
            vh.AddVert(rightBottom, color32, uvs[3]);

            vh.AddTriangle(i, i + 1, i + 2);
            vh.AddTriangle(i + 2, i + 3, i);
        }

        private void GetStrechedBillboardsSizeAndRotation(ParticleSystem.Particle particle, float timeAlive01,
            ref Vector3 size3D, out Quaternion rotation)
        {
            var velocityOverLifeTime = Vector3.zero;

            if (this.ps.velocityOverLifetime.enabled)
            {
                velocityOverLifeTime.x = this.ps.velocityOverLifetime.x.Evaluate(timeAlive01);
                velocityOverLifeTime.y = this.ps.velocityOverLifetime.y.Evaluate(timeAlive01);
                velocityOverLifeTime.z = this.ps.velocityOverLifetime.z.Evaluate(timeAlive01);
            }

            var finalVelocity = particle.velocity + velocityOverLifeTime;
            var ang = Vector3.Angle(finalVelocity, Vector3.up);
            var horizontalDirection = finalVelocity.x < 0 ? 1 : -1;
            rotation = Quaternion.Euler(new Vector3(0, 0, ang * horizontalDirection));
            size3D.y *= this.stretchedLenghScale;
            size3D += new Vector3(0, this.stretchedSpeedScale * finalVelocity.magnitude);
        }

        private void EvaluateQuadUVs(Vector2[] uvs)
        {
            uvs[0] = new Vector2(0f, 0f);
            uvs[1] = new Vector2(0f, 1f);
            uvs[2] = new Vector2(1f, 1f);
            uvs[3] = new Vector2(1f, 0f);
        }

        private void EvaluateTexturesheetUVs(ParticleSystem.Particle particle, float timeAlive, Vector2[] uvs)
        {
            var textureAnimator = this.ps.textureSheetAnimation;

            float lifeTimePerCycle = particle.startLifetime / textureAnimator.cycleCount;
            float timePerCycle = timeAlive % lifeTimePerCycle;
            float timeAliveAnim01 = timePerCycle / lifeTimePerCycle; // in percents


            var totalFramesCount = textureAnimator.numTilesY * textureAnimator.numTilesX;
            var frame01 = textureAnimator.frameOverTime.Evaluate(timeAliveAnim01);

            var frame = 0f;
            switch (textureAnimator.animation)
            {
                case ParticleSystemAnimationType.WholeSheet:
                    {
                        frame = Mathf.Clamp(Mathf.Floor(frame01 * totalFramesCount), 0, totalFramesCount - 1);
                        break;
                    }
                case ParticleSystemAnimationType.SingleRow:
                    {
                        frame = Mathf.Clamp(Mathf.Floor(frame01 * textureAnimator.numTilesX), 0, textureAnimator.numTilesX - 1);
                        int row = textureAnimator.rowIndex;
                        if (textureAnimator.useRandomRow)
                        {
                            Random.InitState((int)particle.randomSeed);
                            row = Random.Range(0, textureAnimator.numTilesY);
                        }
                        frame += row * textureAnimator.numTilesX;
                        break;
                    }
            }

            int x = (int)frame % textureAnimator.numTilesX;
            int y = (int)frame / textureAnimator.numTilesX;


            var xDelta = 1f / textureAnimator.numTilesX;
            var yDelta = 1f / textureAnimator.numTilesY;
            y = textureAnimator.numTilesY - 1 - y;
            var sX = x * xDelta;
            var sY = y * yDelta;
            var eX = sX + xDelta;
            var eY = sY + yDelta;

            uvs[0] = new Vector2(sX, sY);
            uvs[1] = new Vector2(sX, eY);
            uvs[2] = new Vector2(eX, eY);
            uvs[3] = new Vector2(eX, sY);
        }
    }
}
