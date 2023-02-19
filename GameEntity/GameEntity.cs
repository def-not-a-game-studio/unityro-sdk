using Assets.Scripts.Renderer.Sprite;
using ROIO.Models.FileTypes;
using UnityEngine;

public class GameEntity : MonoBehaviour {

    [SerializeField]
    private SpriteViewer SpriteViewer;

    [field: SerializeField] public int Direction { get; private set; }
    [field: SerializeField] public int HeadDir { get; private set; }
    [field: SerializeField] public bool IsMonster { get; private set; }
    [field: SerializeField] public GameEntityStatus Status { get; private set; }

    private GameEntityMovementController MovementController;
    private StrEffectRenderer StrEffectRenderer;

    private void Awake() {
        MovementController = GetComponent<GameEntityMovementController>();
        StrEffectRenderer = GetComponent<StrEffectRenderer>();
    }

    private void Start() {
        var effect = Resources.Load<STR>("Effects/2d/°¢¼º");
        if(effect != null) {
            StrEffectRenderer.Initialize(effect);
        }
    }

    public void ChangeMotion(MotionRequest request) {
        SpriteViewer.ChangeMotion(request);
    }

    public void SetDirection(int direction) {
        Direction = direction;
    }

    public void StartMoving(int x, int y) {
        MovementController.StartMoving(x, y);
    }
}

