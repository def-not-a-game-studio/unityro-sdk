using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityRO.Core.Camera;
using UnityRO.Core.GameEntity;

namespace UnityRO.Core.Sprite {
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
    public class SpriteEffectViewer : MonoBehaviour, ISpriteViewer {
        [field: SerializeField] public CoreSpriteGameEntity Entity { get; private set; }
        [field: SerializeField] public ViewerType ViewerType { get; private set; }
        [field: SerializeField] public EntityState State { get; private set; }
        [field: SerializeField] public SpriteMotion Motion { get; private set; }

        [SerializeField] private SpriteData SpriteData;
        [SerializeField] private Texture2D Atlas;
        [SerializeField] private List<SpriteViewer> Children = new();
        [SerializeField] private SpriteViewer Parent;

        private Dictionary<ACT.Frame, Mesh> ColliderCache = new();
        private Dictionary<ACT.Frame, Mesh> MeshCache = new();

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

        void Update() {
            if (SpriteData == null) return;
            
            var frame = UpdateFrame();
            UpdateMesh(frame);
            UpdateLocalPosition();
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

            MeshRenderer.material.SetFloat(UsePaletteProp, SpriteData.palettes.Length);
            if (SpriteData.palettes.Length <= 0) {
                Atlas.filterMode = FilterMode.Bilinear;
            }

            MeshRenderer.material.SetTexture(MainTexProp, Atlas);

            if (SpriteData.palettes.Length > 0) {
                MeshRenderer.material.SetTexture(PaletteTexProp, SpriteData.palettes[0]);
            }
        }

        public Vector2 GetAnimationAnchor() {
            var frame = UpdateFrame();

            return frame.pos.Length > 0 ? frame.pos[0] : Vector2.zero;
        }

        private void UpdateLocalPosition() {
            if (Parent == null)
                return;

            var parentAnchor = Parent.GetAnimationAnchor();
            var ourAnchor = GetAnimationAnchor();

            if (ourAnchor == Vector2.zero) {
                MeshRenderer.material.SetVector(OffsetProp, Vector3.zero);
                return;
            }

            var diff = parentAnchor - ourAnchor;
            var localPosition = new Vector3(diff.x, -diff.y, 0f) / SPR.PIXELS_PER_UNIT;
            MeshRenderer.material.SetVector(OffsetProp, localPosition);
        }

        private ACT.Frame UpdateFrame() {
            return  FramePaceCalculator.GetCurrentFrame();
        }

        private void UpdateMesh(ACT.Frame frame) {
            // We need this mesh collider in order to have the raycast to hit the sprite
            ColliderCache.TryGetValue(frame, out Mesh colliderMesh);
            if (colliderMesh == null) {
                colliderMesh = SpriteMeshBuilder.BuildColliderMesh(frame, Sprites);
                ColliderCache.Add(frame, colliderMesh);
            }

            MeshCache.TryGetValue(frame, out Mesh rendererMesh);
            if (rendererMesh == null) {
                rendererMesh = SpriteMeshBuilder.BuildSpriteMesh(frame, Sprites, 0);
                MeshCache.Add(frame, rendererMesh);
            }

            MeshFilter.sharedMesh = null;
            MeshFilter.sharedMesh = rendererMesh;
            MeshCollider.sharedMesh = colliderMesh;
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

        private void SetAlpha(float alpha) {
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
            if (ViewerType == ViewerType.Emotion) {
                Destroy(gameObject);
            }
        }
    }
}