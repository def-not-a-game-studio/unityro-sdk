using ROIO.Models.FileTypes;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
public class SpriteViewer : MonoBehaviour {
    [field: SerializeField] public CoreSpriteGameEntity Entity { get; private set; }
    [field: SerializeField] public ViewerType ViewerType { get; private set; }
    [field: SerializeField] public SpriteState State { get; private set; }

    [SerializeField] private SpriteData SpriteData;

    [SerializeField] private Texture2D Atlas;

    [SerializeField] private SpriteViewer[] Children;

    [SerializeField] private SpriteViewer Parent;

    private Dictionary<ACT.Frame, Mesh> ColliderCache = new Dictionary<ACT.Frame, Mesh>();
    private Dictionary<ACT.Frame, Mesh> MeshCache = new Dictionary<ACT.Frame, Mesh>();

    private MeshRenderer MeshRenderer;
    private MeshFilter MeshFilter;
    private MeshCollider MeshCollider;

    private UnityEngine.Sprite[] Sprites;

    private int ActionId;
    private ACT.Action CurrentAction;
    private int CurrentFrameIndex;
    private FramePaceCalculator FramePaceCalculator;

    private void Awake() {
        InitializeRenderers();
    }

    private void Start() {
        ChangeMotion(new MotionRequest { Motion = SpriteMotion.Standby });
    }

    private void Update() {
        var frame = UpdateFrame();
        UpdateMesh(frame);
        UpdateLocalPosition();
    }

    public void ChangeMotion(MotionRequest motion, MotionRequest? nextMotion = null) {
        var state = motion.Motion switch {
                        SpriteMotion.Idle => SpriteState.Idle,
                        SpriteMotion.Standby => SpriteState.Standby,
                        SpriteMotion.Walk => SpriteState.Walking,
                        _ => throw new System.NotImplementedException()
                    };

        if (state == State) {
            return;
        }

        State = state;
        ActionId = AnimationHelper.GetMotionIdForSprite(motion.Motion, Entity.IsMonster);
        CurrentFrameIndex = 0;

        FramePaceCalculator.OnMotionChanged(motion, nextMotion, ActionId);

        foreach (var child in Children) {
            child.ChangeMotion(motion, nextMotion);
        }
    }

    public Vector2 GetAnimationAnchor() {
        if (CurrentAction == null) {
            return Vector2.zero;
        }

        var frame = CurrentAction.frames[CurrentFrameIndex];
        if (frame.pos.Length > 0)
            return frame.pos[0];
        if (ViewerType == ViewerType.Head && State == SpriteState.Idle)
            return frame.pos[CurrentFrameIndex];
        return Vector2.zero;
    }

    private void InitializeRenderers() {
        MeshRenderer = GetComponent<MeshRenderer>();
        MeshFilter = GetComponent<MeshFilter>();
        MeshCollider = GetComponent<MeshCollider>();
        Entity ??= GetComponentInParent<CoreSpriteGameEntity>();

        Sprites = SpriteData.sprites;
        FramePaceCalculator = new FramePaceCalculator(Entity, ViewerType, SpriteData.act);
        MeshRenderer.material = new Material(Shader.Find("Shaders/SpriteShaderGraph"));
        MeshRenderer.material.SetTexture("_MainTex", Atlas);
    }

    private void UpdateLocalPosition() {
        if (Parent == null)
            return;

        var parentAnchor = Parent.GetAnimationAnchor();
        var ourAnchor = GetAnimationAnchor();

        var diff = parentAnchor - ourAnchor;

        transform.localPosition = new Vector3(diff.x, -diff.y, 0f) / 32;
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
}

public enum ViewerType {
    Head,
    Body
}

public enum SpriteState {
    Idle,
    Walking,
    Standby
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