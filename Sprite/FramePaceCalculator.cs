using System.Collections;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityRO.Core.Camera;

public class FramePaceCalculator {
    private const int AVERAGE_ATTACK_SPEED = 432;
    private const int AVERAGE_ATTACKED_SPEED = 288;
    private const int MAX_ATTACK_SPEED = AVERAGE_ATTACKED_SPEED * 2;

    [SerializeField] private CoreSpriteGameEntity Entity;
    [SerializeField] private ViewerType ViewerType;
    [SerializeField] private int CurrentFrame = 0;
    [SerializeField] private long AnimationStart = GameManager.Tick;
    [SerializeField] private float CurrentDelay = 0f;
    [SerializeField] private MotionRequest CurrentMotion;
    [SerializeField] private MotionRequest? NextMotion;
    [SerializeField] private ACT CurrentACT;
    [SerializeField] private ACT.Action CurrentAction;
    [SerializeField] private int ActionId;

    private CharacterCamera CharacterCamera;

    private Coroutine MotionQueueCoroutine;

    public FramePaceCalculator(CoreSpriteGameEntity entity,
        ViewerType viewerType,
        ACT currentACT,
        CharacterCamera characterCamera) {
        Entity = entity;
        ViewerType = viewerType;
        CurrentACT = currentACT;
        CharacterCamera = characterCamera;
    }

    public int GetActionIndex() {
        var cameraDirection = (int)(CharacterCamera?.Direction ?? 0);
        var entityDirection = (int)Entity.Direction + 8;

        return (ActionId + (cameraDirection + entityDirection) % 8) % CurrentACT.actions.Length;
    }

    public int GetCurrentFrame() {
        CurrentAction = CurrentACT.actions[GetActionIndex()];

        var isIdle = (Entity.Status.EntityType == EntityType.PC && CurrentMotion.Motion is SpriteMotion.Idle or SpriteMotion.Sit);
        int frameCount = CurrentAction.frames.Length;
        long deltaSinceMotionStart = GameManager.Tick - AnimationStart;

        var maxFrame = frameCount - 1;

        if (isIdle) {
            CurrentFrame = Entity.HeadDirection;
        }

        CurrentDelay = GetDelay();
        if (deltaSinceMotionStart >= CurrentDelay) {
            AnimationStart = GameManager.Tick;

            if (CurrentFrame < maxFrame && !isIdle) {
                CurrentFrame++;
            }
        }

        if (CurrentFrame >= maxFrame) {
            if (AnimationHelper.IsLoopingMotion(CurrentMotion.Motion)) {
                CurrentFrame = 0;
            } else if (NextMotion.HasValue && ViewerType == ViewerType.Body) {
                // Since body is the main component, it's the only one "allowed" to ask for the next motion
                Entity.ChangeMotion(NextMotion.Value);
            } else {
                CurrentFrame = maxFrame;
            }
        }

        return CurrentFrame;
    }

    public float GetDelay() {
        if (ViewerType == ViewerType.Body && CurrentMotion.Motion == SpriteMotion.Walk) {
            return CurrentAction.delay / 150 * Entity.Status.MoveSpeed;
        }

        if (CurrentMotion.Motion is SpriteMotion.Attack or SpriteMotion.Attack1 or SpriteMotion.Attack2 or SpriteMotion.Attack3) {
            return (float)Entity.Status.AttackSpeed / CurrentAction.frames.Length;
        }

        return CurrentAction.delay;
    }

    private IEnumerator DelayCurrentMotion(MotionRequest currentMotion, MotionRequest? nextMotion, int actionId) {
        yield return new WaitUntil(() => GameManager.Tick > currentMotion.delay);
        OnMotionChanged(currentMotion, nextMotion, actionId);
    }

    public void OnMotionChanged(MotionRequest currentMotion, MotionRequest? nextMotion, int actionId) {
        if (MotionQueueCoroutine != null) {
            Entity.StopCoroutine(MotionQueueCoroutine);
            MotionQueueCoroutine = null;
        }

        if (currentMotion.delay > GameManager.Tick) {
            MotionQueueCoroutine = Entity.StartCoroutine(DelayCurrentMotion(currentMotion, nextMotion, actionId));
            return;
        }

        AnimationStart = GameManager.Tick;
        CurrentFrame = 0;
        CurrentMotion = currentMotion;
        NextMotion = nextMotion;
        ActionId = actionId;
    }
}