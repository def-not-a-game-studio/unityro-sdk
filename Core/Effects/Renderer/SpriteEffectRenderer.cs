using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _3rdparty.unityro_sdk.Core.Effects;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityRO.Core;
using UnityRO.Core.Camera;
using UnityRO.Core.GameEntity;
using UnityRO.Core.Sprite;

namespace Core.Effects
{
    public class SpriteEffectRenderer : EffectRendererPart, ISpriteViewer
    {
        public CoreSpriteGameEntity Entity { get; private set; }
        public ViewerType ViewerType { get; private set; }
        public EntityState State { get; private set; }
        public SpriteMotion Motion { get; private set; }

        private SpriteData SpriteData;
        private Texture2D Atlas;
        private AudioClip AudioClip;

        private Dictionary<int, Mesh> MeshCache = new();
        public Sprite[] Sprites;

        private Material _material;
        private Mesh _currentMesh;
        private RenderParams _renderParams;

        private FramePaceCalculator FramePaceCalculator;

        private static readonly int MainTexProp = Shader.PropertyToID("_MainTex");
        private static readonly int AlphaProp = Shader.PropertyToID("_Alpha");
        private static readonly int OffsetProp = Shader.PropertyToID("_Offset");

        public void Init(SpriteData spriteData, AudioClip wav, ViewerType viewerType, CoreSpriteGameEntity entity)
        {
            SpriteData = spriteData;
            Atlas = spriteData.atlas;
            ViewerType = viewerType;
            Entity = entity;

            InitializeRenderers();
        }

        public override void Update(Matrix4x4 matrix)
        {
            if (SpriteData is null) return;

            var frame = UpdateFrame();
            UpdateMesh(frame);

            Graphics.RenderMesh(_renderParams, _currentMesh, 0, matrix);
        }

        private void InitializeRenderers()
        {
            if (SpriteData is null) return;

            Sprites = SpriteData.GetSprites();
            FramePaceCalculator = new FramePaceCalculator(Entity, this, SpriteData.act);
            _material = Resources.Load<Material>("Materials/SpriteEffects");
            _material.SetFloat(AlphaProp, 1f);
            _material.SetTexture(MainTexProp, Atlas);
            _material.SetVector(OffsetProp, new Vector3(0, 3, 0));
            _renderParams = new RenderParams(_material)
            {
                receiveShadows = false,
                lightProbeUsage = LightProbeUsage.Off
            };
        }

        private ACT.Frame UpdateFrame() => FramePaceCalculator.GetCurrentFrame();

        private void UpdateMesh(ACT.Frame frame)
        {
            MeshCache.TryGetValue(frame.id, out var rendererMesh);
            if (rendererMesh is null)
            {
                rendererMesh = SpriteMeshBuilder.BuildSpriteMesh(frame, Sprites);
                MeshCache.Add(frame.id, rendererMesh);
            }

            _currentMesh = rendererMesh;
        }

        public void SetActionIndex(int actionIndex)
        {
            FramePaceCalculator.SetActionIndex(actionIndex);
        }

        public ViewerType GetViewerType() => ViewerType;

        public void OnAnimationFinished()
        {
            if (ViewerType is ViewerType.Emotion or ViewerType.Effect)
            {
                OnEnd?.Invoke(this);
            }
        }
    }
}