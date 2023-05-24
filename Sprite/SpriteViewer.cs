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
    public class SpriteViewer : ManagedMonoBehaviour {
        [field: SerializeField] public CoreSpriteGameEntity Entity { get; private set; }
        [field: SerializeField] public ViewerType ViewerType { get; private set; }
        [field: SerializeField] public SpriteState State { get; private set; }

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

        private int ActionId;
        private ACT.Action CurrentAction;
        private int CurrentFrameIndex;
        private FramePaceCalculator FramePaceCalculator;
        
        private static readonly int OffsetProp = Shader.PropertyToID("_Offset");
        private static readonly int UsePaletteProp = Shader.PropertyToID("_UsePalette");
        private static readonly int MainTexProp = Shader.PropertyToID("_MainTex");
        private static readonly int PaletteTexProp = Shader.PropertyToID("_PaletteTex");

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
            ChangeMotion(new MotionRequest { Motion = SpriteMotion.Idle });
        }

        public void SetParent(SpriteViewer parent) {
            Parent = parent;
        }

        public void AddChildren(SpriteViewer child) {
            Children.Add(child);
        }

        public override void ManagedUpdate() {
            var frame = UpdateFrame();
            UpdateMesh(frame);
            UpdateLocalPosition();
        }

        public void ChangeMotion(MotionRequest motion, MotionRequest? nextMotion = null) {
            var state = motion.Motion switch {
                            SpriteMotion.Idle => SpriteState.Idle,
                            SpriteMotion.Standby => SpriteState.Standby,
                            SpriteMotion.Walk => SpriteState.Walking,
                            SpriteMotion.Attack => SpriteState.Attack,
                            SpriteMotion.Attack1 => SpriteState.Attack,
                            SpriteMotion.Attack2 => SpriteState.Attack,
                            SpriteMotion.Attack3 => SpriteState.Attack,
                            SpriteMotion.Dead => SpriteState.Dead,
                            SpriteMotion.Hit => SpriteState.Hit,
                            SpriteMotion.Casting => SpriteState.Casting,
                            SpriteMotion.PickUp => SpriteState.PickUp,
                            SpriteMotion.Freeze1 => SpriteState.Frozen,
                            SpriteMotion.Freeze2 => SpriteState.Frozen,
                            _ => SpriteState.Idle
                        };

            if (state == State) {
                return;
            }

            if (state == SpriteState.Dead) {
                MeshRenderer.material.SetShaderPassEnabled("ShadowCaster", false);
            } else {
                MeshRenderer.material.SetShaderPassEnabled("ShadowCaster", true);
            }

            State = state;
            ActionId = AnimationHelper.GetMotionIdForSprite(Entity.Status.EntityType, motion.Motion);
            CurrentFrameIndex = 0;

            FramePaceCalculator.OnMotionChanged(motion, nextMotion, ActionId);

            foreach (var child in Children) {
                child.ChangeMotion(motion, nextMotion);
            }
        }

        public void UpdatePalette() {
            if (SpriteData.palettes.Length <= 0) return;
            var palette = ViewerType switch {
                              ViewerType.Head => SpriteData.palettes[Entity.Status.HairColor],
                              ViewerType.Body => SpriteData.palettes[Entity.Status.ClothesColor],
                              _ => throw new ArgumentOutOfRangeException()
                          };

            if (palette != null) {
                MeshRenderer.material.SetTexture("_PaletteTex", palette);
            }
        }

        private void InitializeRenderers() {
            MeshRenderer = GetComponent<MeshRenderer>();
            MeshFilter = GetComponent<MeshFilter>();
            MeshCollider = GetComponent<MeshCollider>();
            Entity ??= GetComponentInParent<CoreSpriteGameEntity>();

            Sprites = SpriteData.GetSprites();
            FramePaceCalculator = new FramePaceCalculator(Entity, ViewerType, SpriteData.act, CharacterCamera);
            MeshRenderer.material = Resources.Load<Material>("Materials/BillboardSpriteMaterial");


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
            if (CurrentAction == null) {
                return Vector2.zero;
            }

            var frame = CurrentAction.frames[CurrentFrameIndex];

            if (ViewerType == ViewerType.Head && (State == SpriteState.Idle || State == SpriteState.Sit))
                return frame.pos[CurrentFrameIndex];

            return frame.pos.Length > 0 ? frame.pos[0] : Vector2.zero;
        }

        private void UpdateLocalPosition() {
            if (Parent == null)
                return;

            var parentAnchor = Parent.GetAnimationAnchor();
            var ourAnchor = GetAnimationAnchor();

            if (ourAnchor == Vector2.zero) {
                transform.localPosition = new Vector3(0f, 0f, 0f);
                return;
            }

            var diff = parentAnchor - ourAnchor;
            var localPosition = new Vector3(diff.x, -diff.y, 0f) / SPR.PIXELS_PER_UNIT;
            MeshRenderer.material.SetVector(OffsetProp, localPosition);
        }

        private ACT.Frame UpdateFrame() {
            CurrentAction = SpriteData.act.actions[FramePaceCalculator.GetActionIndex()];
            CurrentFrameIndex = FramePaceCalculator.GetCurrentFrame();
            var frame = CurrentAction.frames[CurrentFrameIndex];
            return frame;
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
                rendererMesh = SpriteMeshBuilder.BuildSpriteMesh(frame, Sprites);
                MeshCache.Add(frame, rendererMesh);
            }

            foreach (var layer in frame.layers) {
                MeshRenderer.material.SetFloat("_Alpha", layer.color.a);
            }

            MeshFilter.sharedMesh = null;
            MeshFilter.sharedMesh = rendererMesh;
            MeshCollider.sharedMesh = colliderMesh;
        }

        public SpriteViewer FindChild(ViewerType viewerType) {
            return Children.FirstOrDefault(it => it.ViewerType == viewerType);
        }

        public void FadeOut() {
            StartCoroutine(FadeOutRenderer());
        }

        private IEnumerator FadeOutRenderer() {
            float currentTime = 0f;
            var currentAlpha = MeshRenderer.material.GetFloat("_Alpha");

            while (currentTime <= 0.5f) {
                currentTime += Time.deltaTime;
                currentAlpha = Mathf.Lerp(currentAlpha, 0f, Time.deltaTime / 0.5f);
                MeshRenderer.material.SetFloat("_Alpha", currentAlpha);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public enum ViewerType {
        Head,
        Body
    }

    public enum SpriteState {
        Idle,
        Walking,
        Standby,
        Dead,
        Hit,
        Attack,
        Casting,
        PickUp,
        Frozen,
        Sit
    }

    public enum SpriteMotion {
        Idle,
        Walk,
        Sit,
        PickUp,
        Attack,
        Attack1, // Normal attack
        Attack2, // No weapon attack
        Attack3, // Combo attack
        Standby,
        Hit,
        Freeze1,
        Freeze2,
        Dead,
        Casting,
        Special,
        Performance1,
        Performance2,
        Performance3,
    }
}