using Assets.Scripts.Renderer.Sprite;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEntityMovementController : MonoBehaviour {
    private GameEntity Entity;
    private PathFinder PathFinder;

    [SerializeField] private Joystick Joystick;

    #region Behaviour

    [SerializeField] private int MoveSpeedDivider = 18;

    [field: SerializeField] public bool HasAuthority { get; private set; } = false;

    private bool IsWalking;
    private int NodeIndex;
    private List<Vector3> Nodes = new();

    #endregion

    private void Awake() {
        Entity = GetComponent<GameEntity>();

        if (!HasAuthority) {
            return;
        }

        Joystick.SnapX = true;
        Joystick.SnapY = true;
    }

    private void Start() {
        PathFinder = FindObjectOfType<PathFinder>();
    }

    void Update() {
        ProcessInput();

        if (IsWalking && Nodes.Count > 0) {
            if (NodeIndex == Nodes.Count - 1) {
                StopMoving();
                return;
            }

            var current = Nodes[NodeIndex];
            var next = Nodes[NodeIndex + 1];
            var direction = (next - current).normalized;

            var nextPosition = (Entity.Status.walkSpeed / MoveSpeedDivider) * Time.deltaTime * direction;
            transform.position += nextPosition;

            if (transform.position.x >= next.x && transform.position.z >= next.z) {
                NodeIndex++;
            }
        }
    }

    private void ProcessInput() {
        if (!HasAuthority) {
            return;
        }

        var vertical = Joystick.Vertical;
        var horizontal = Joystick.Horizontal;

        var dir = new Vector3(horizontal, 0f, vertical);

        if (dir == Vector3.zero) {
            StopMoving();
        } else {
            var position = transform.position;
            StartMoving(
                        startX: Mathf.FloorToInt(position.x),
                        startY: Mathf.FloorToInt(position.z),
                        endX: Mathf.FloorToInt(position.x + dir.x),
                        endY: Mathf.FloorToInt(position.z + dir.z),
                        dir: dir);
        }
    }

    public void StartMoving(int x, int y) {
        var destination = new Vector3(x, PathFinder.GetCellHeight(x, y), y);
        var position = transform.position;
        var direction = (destination - position).normalized;

        if (direction != Vector3.zero) {
            StartMoving(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z), x, y, direction);
        } else {
            StopMoving();
        }
    }

    /// <summary>
    /// Server has acknowledged our request and we're good to go.
    /// Use to bypass server request (ie: Offline mode)
    /// </summary>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <param name="endX"></param>
    /// <param name="endY"></param>
    public void StartMoving(
        int startX, int startY, int endX,
        int endY, Vector3 dir
    ) {
        NodeIndex = 0;
        Nodes = PathFinder
                .GetPath(startX, startY, endX, endY)
                .Select(node => new Vector3(node.x, (float)node.y, node.z))
                .ToList();

        if (Nodes.Count <= 0) return;
        IsWalking = true;
        Entity.ChangeMotion(new MotionRequest { Motion = SpriteMotion.Walk });
        Entity.SetDirection((int)GetDirection(dir));
    }

    /// <summary>
    /// Stops moving the character.
    /// Clear the path finder nodes and set state back to Wait
    /// </summary>
    public void StopMoving() {
        IsWalking = false;
        Nodes.Clear();
        Entity.ChangeMotion(new MotionRequest { Motion = SpriteMotion.Standby });
        return;
    }

    private Direction GetDirection(Vector3 dir) => GetDirectionForOffset(new Vector2Int((int)dir.x, (int)dir.z));

    private Direction GetDirectionForOffset(Vector2Int offset) {
        return offset.x switch {
                   -1 when offset.y == -1 => Direction.SouthWest,
                   -1 when offset.y == 0 => Direction.West,
                   -1 when offset.y == 1 => Direction.NorthWest,
                   0 when offset.y == 1 => Direction.North,
                   1 when offset.y == 1 => Direction.NorthEast,
                   1 when offset.y == 0 => Direction.East,
                   1 when offset.y == -1 => Direction.SouthEast,
                   0 when offset.y == -1 => Direction.South,
                   _ => Direction.South
               };
    }
}