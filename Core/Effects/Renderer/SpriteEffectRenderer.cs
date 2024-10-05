using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _3rdparty.unityro_sdk.Core.Effects;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityRO.Core;
using UnityRO.Core.Camera;
using UnityRO.Core.GameEntity;
using UnityRO.Core.Sprite;

namespace Core.Effects
{
    public class SpriteEffectRenderer : ManagedMonoBehaviour, ISpriteViewer
    {
        [field: SerializeField] public CoreSpriteGameEntity Entity { get; private set; }
        [field: SerializeField] public ViewerType ViewerType { get; private set; }
        [field: SerializeField] public EntityState State { get; private set; }
        [field: SerializeField] public SpriteMotion Motion { get; private set; }

        [SerializeField] private SpriteData SpriteData;
        [SerializeField] private Texture2D Atlas;
        [SerializeField] private AudioClip AudioClip;

        private Dictionary<int, Mesh> MeshCache = new();
        private CharacterCamera CharacterCamera;
        public Sprite[] Sprites;

        private Material _material;
        private Mesh _currentMesh;
        private RenderParams _renderParams;

        private FramePaceCalculator FramePaceCalculator;

        private static readonly int MainTexProp = Shader.PropertyToID("_MainTex");
        private static readonly int AlphaProp = Shader.PropertyToID("_Alpha");

        public void Init(SpriteData spriteData, AudioClip wav, ViewerType viewerType)
        {
            SpriteData = spriteData;
            Atlas = spriteData.atlas;
            ViewerType = viewerType;
            AudioClip = wav;

            Entity = gameObject.AddComponent<EffectGameEntity>();

            InitializeRenderers();
        }

        public void Init(SpriteData spriteData, ViewerType viewerType, CoreSpriteGameEntity entity)
        {
            SpriteData = spriteData;
            Atlas = spriteData.atlas;
            ViewerType = viewerType;
            Entity = entity;

            InitializeRenderers();
        }

        private void Awake()
        {
            CharacterCamera = FindObjectOfType<CharacterCamera>();
        }

        private void Start()
        {
            InitializeRenderers();
        }

        public override void ManagedUpdate()
        {
            if (SpriteData is null) return;

            var frame = UpdateFrame();
            UpdateMesh(frame);

            Graphics.RenderMesh(_renderParams, _currentMesh, 0, transform.localToWorldMatrix);
        }

        private void InitializeRenderers()
        {
            Entity ??= GetComponentInParent<CoreSpriteGameEntity>();

            if (SpriteData is null) return;

            Sprites = SpriteData.GetSprites();
            FramePaceCalculator = new FramePaceCalculator(Entity, this, SpriteData.act, CharacterCamera);
            _material = Resources.Load<Material>("Materials/SpriteEffects");
            _material.SetFloat(AlphaProp, 1f);
            _material.SetTexture(MainTexProp, Atlas);
            _renderParams = new RenderParams(_material);
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
                Destroy(gameObject);
            }
        }
    }
}