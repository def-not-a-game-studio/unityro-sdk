using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityRO.Core.Camera;
using UnityRO.Core.Database;
using UnityRO.Core.GameEntity;

namespace UnityRO.Core.Sprite {
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
    public partial class SpriteViewer : MonoBehaviour, ISpriteViewer
    {
        public bool IsReady { get; private set; } = false;
        
        [field: SerializeField] public CoreSpriteGameEntity Entity { get; private set; }
        [field: SerializeField] public ViewerType ViewerType { get; private set; }

        [SerializeField] private SpriteData SpriteData;
        [SerializeField] private Texture2D Atlas;
        [SerializeField] private List<SpriteViewer> Children = new();
        [SerializeField] private SpriteViewer Parent;
        
        private Dictionary<int, Mesh> ColliderCache = new();
        private Dictionary<int, Mesh> MeshCache = new();

        private MeshRenderer MeshRenderer;
        private MeshFilter MeshFilter;
        private MeshCollider MeshCollider;
        private CharacterCamera CharacterCamera;

        public UnityEngine.Sprite[] Sprites;

        private FramePaceCalculator FramePaceCalculator;

        private static readonly int OffsetProp = Shader.PropertyToID("_Offset");
        private static readonly int UsePaletteProp = Shader.PropertyToID("_UsePalette");
        private static readonly int MainTexProp = Shader.PropertyToID("_MainTex");
        private static readonly int PaletteTexProp = Shader.PropertyToID("_PaletteTex");
        private static readonly int AlphaProp = Shader.PropertyToID("_Alpha");
        private static readonly int ColorProp = Shader.PropertyToID("_Color");
        
        public void Init(SpriteData spriteData, ViewerType viewerType, CoreSpriteGameEntity entity) {
            SpriteData = spriteData;
            Atlas = spriteData.atlas;
            ViewerType = viewerType;
            Entity = entity;

            InitializeRenderers();
        }

        private void Awake() {
            CharacterCamera = FindObjectOfType<CharacterCamera>();
        }

        private void Start() {
            InitializeRenderers();
        }

        public void SetParent(SpriteViewer parent) {
            Parent = parent;
        }

        public void AddChildren(SpriteViewer child) {
            Children.Add(child);
        }

        public void ManagedUpdate() {
            if (SpriteData == null) return;

            UpdateFrames();
        }
        
        public void UpdatePalette() {
            if (SpriteData.palettes.Length <= 0) return;
            var palette = ViewerType switch {
                              ViewerType.Head => SpriteData.palettes[Entity.Status.HairColor],
                              ViewerType.Body => SpriteData.palettes[Entity.Status.ClothesColor],
                              _ => throw new ArgumentOutOfRangeException()
                          };

            if (palette != null) {
                MeshRenderer.material.SetTexture(PaletteTexProp, palette);
            }
        }

        private void InitializeRenderers() {
            MeshRenderer = GetComponent<MeshRenderer>();
            MeshFilter = GetComponent<MeshFilter>();
            MeshCollider = GetComponent<MeshCollider>();
            Entity ??= GetComponentInParent<CoreSpriteGameEntity>();

            if (SpriteData == null) return;

            Sprites = SpriteData.GetSprites();
            FramePaceCalculator = new FramePaceCalculator(Entity, this, SpriteData.act, CharacterCamera);

            MeshRenderer.material = Resources.Load<Material>("Materials/BillboardSpriteMaterial");
            MeshRenderer.material.SetFloat(AlphaProp, 1f);

            var rendererIndex = ViewerType switch {
                ViewerType.Head => 2,
                ViewerType.Body => 1,
                ViewerType.Emotion => 0,
                _ => 0
            };
            MeshRenderer.material.renderQueue += rendererIndex;
            
            // if palettes are present, we blit the palette into a render texture then
            // apply filtering to the renderTexture so it looks better
            if (SpriteData.palettes.Length > 0)
            {
                var material = Resources.Load<Material>("Materials/Palette");
                var renderTexture = RenderTexture.GetTemporary(Atlas.width, Atlas.height, 0, RenderTextureFormat.ARGB32);
                renderTexture.filterMode = FilterMode.Trilinear;
                material.SetFloat(UsePaletteProp, SpriteData.palettes.Length);
                if (SpriteData.palettes.Length > 0) {
                    material.SetFloat(UsePaletteProp, SpriteData.palettes.Length);
                    material.SetTexture(PaletteTexProp, SpriteData.palettes[0]);
                }
                Graphics.Blit(Atlas, renderTexture, material);
                
                MeshRenderer.material.SetTexture(MainTexProp, renderTexture);
            }
            else
            {
                Atlas.filterMode = FilterMode.Trilinear;
                MeshRenderer.material.SetTexture(MainTexProp, Atlas);
            }
            
            IsReady = true;
        }

        public SpriteViewer FindChild(ViewerType viewerType) {
            return Children.FirstOrDefault(it => it.ViewerType == viewerType);
        }
        
        public void FadeOut(float delay = 2f, float timeout = 2f) {
            StartCoroutine(FadeOutRenderer(delay, timeout));
        }

        public IEnumerator FadeOutRenderer(float delay, float timeout) {
            yield return new WaitForSeconds(delay);
            var currentTime = 0f;
            var currentAlpha = MeshRenderer.material.GetFloat(AlphaProp);

            while (currentTime <= timeout && currentAlpha > 0f) {
                currentTime += Time.deltaTime;
                currentAlpha = Mathf.Lerp(currentAlpha, 0f, currentTime / timeout);
                SetAlpha(currentAlpha);
                foreach (var child in Children) {
                    child.SetAlpha(currentAlpha);
                }

                yield return null;
            }
        }

        public void SetAlpha(float alpha) {
            MeshRenderer.material.SetFloat(AlphaProp, alpha);
        }

        public void Teardown() {
            SpriteData = null;
            Atlas = null;
            Sprites = null;
            foreach (var child in Children) {
                child.Teardown();
            }
        }

        public void SetActionIndex(int actionIndex) {
            FramePaceCalculator.SetActionIndex(actionIndex);
        }

        public ViewerType GetViewerType() => ViewerType;

        public void OnAnimationFinished() {
            if (Entity.State is EntityState.Dead or EntityState.Freeze or EntityState.Sit) {
                return;
            }

            var request = Entity.State switch {
                              EntityState.Hit => new MotionRequest { Motion = SpriteMotion.Standby },
                              EntityState.Attack => new MotionRequest { Motion = SpriteMotion.Standby },
                              _ => new MotionRequest { Motion = SpriteMotion.Idle }
                          };

            Entity.ChangeMotion(request);
        }
    }
}