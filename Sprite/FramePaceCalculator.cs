using System.Collections;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityRO.Core.Camera;
using UnityRO.Core.GameEntity;

namespace UnityRO.Core.Sprite {
    public class FramePaceCalculator {
        private const int AVERAGE_ATTACK_SPEED = 432;
        private const int AVERAGE_ATTACKED_SPEED = 288;
        private const int MAX_ATTACK_SPEED = AVERAGE_ATTACKED_SPEED * 2;

        private CoreSpriteGameEntity Entity;
        private ViewerType ViewerType;
        private int CurrentFrame = 0;
        private long AnimationStart = GameManager.Tick;
        private float CurrentDelay = 0f;
        private MotionRequest CurrentMotion;
        private MotionRequest? NextMotion;
        private ACT CurrentACT;
        private ACT.Action CurrentAction;
        private int ActionId;

        private CharacterCamera CharacterCamera;

        private Coroutine MotionQueueCoroutine;

        public FramePaceCalculator(
            CoreSpriteGameEntity entity,
            ViewerType viewerType,
            ACT currentACT,
            CharacterCamera characterCamera
        ) {
            Entity = entity;
            ViewerType = viewerType;
            CurrentACT = currentACT;
            CharacterCamera = characterCamera;
        }

        public int GetActionIndex() {
            var cameraDirection = (int)CharacterCamera.Direction;
            var entityDirection = (int)Entity.Direction + 8;

            return (ActionId + (cameraDirection + entityDirection) % 8) % CurrentACT.actions.Length;
        }

        public int GetCurrentFrame() {
            CurrentAction = CurrentACT.actions[GetActionIndex()];

            var isIdle = (Entity.Status.EntityType == EntityType.PC &&
                          CurrentMotion.Motion is SpriteMotion.Idle or SpriteMotion.Sit);
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

        private float attackMotion = 6f;

        public float GetDelay() {
            if (ViewerType == ViewerType.Body && CurrentMotion.Motion == SpriteMotion.Walk) {
                return CurrentAction.delay / 150 * Entity.Status.MoveSpeed;
            }

            if (CurrentMotion.Motion is SpriteMotion.Attack
                or SpriteMotion.Attack1
                or SpriteMotion.Attack2
                or SpriteMotion.Attack3) {
                var attackSpeed = (Entity.Status.AttackSpeed > MAX_ATTACK_SPEED ? MAX_ATTACK_SPEED : Entity.Status.AttackSpeed);
                var multiplier = attackSpeed / (float)AVERAGE_ATTACK_SPEED;
                var delayTime = attackMotion * multiplier * 24f;
                if (delayTime < 0) {
                    delayTime = 0;
                }

                return delayTime;
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

            if (CurrentMotion.Motion is SpriteMotion.Attack
                or SpriteMotion.Attack1
                or SpriteMotion.Attack2
                or SpriteMotion.Attack3) {
                if ((EntityType)Entity.GetEntityType() == EntityType.PC) {
                    // switch (isSecondAttack)
                    // {
                    //     case 0:
                    //         switch (job)
                    //         {
                    //             case JT_THIEF:
                    //                 return 5.75f;
                    //                 break;
                    //             case JT_MERCHANT:
                    //                 return 5.85f;
                    //                 break;
                    //         }
                    //         break;
                    //     case 1:
                    //         switch (job)
                    //         {
                    //             case JT_NOVICE:
                    //             case JT_SUPERNOVICE:
                    //             case JT_SUPERNOVICE_B:
                    //                 switch (sex)
                    //                 {
                    //                     case SEX_MALE:
                    //                         return 5.85f;
                    //                         break;
                    //                 }
                    //                 break;
                    //             case JT_ASSASSIN:
                    //             case JT_ASSASSIN_H:
                    //             case JT_ASSASSIN_B:
                    //                 switch (weapon)
                    //                 {
                    //                     case WEAPONTYPE_CATARRH:
                    //                     case WEAPONTYPE_SHORTSWORD_SHORTSWORD:
                    //                     case WEAPONTYPE_SWORD_SWORD:
                    //                     case WEAPONTYPE_AXE_AXE:
                    //                     case WEAPONTYPE_SHORTSWORD_SWORD:
                    //                     case WEAPONTYPE_SHORTSWORD_AXE:
                    //                     case WEAPONTYPE_SWORD_AXE:
                    //                         return 3.0f;
                    //                 }
                    //                 break;
                    //         }
                    //         break;
                    // }
                    // return 6;
                    attackMotion = Entity.Status.attackMotion; // 4f
                } else {
                    for (var index = 0; index < CurrentACT.sounds.Length; index++) {
                        var sound = CurrentACT.sounds[index];
                        if (sound == "atk") {
                            attackMotion = index;
                        }
                    }
                }
            }

            AnimationStart = GameManager.Tick;
            CurrentFrame = 0;

            CurrentMotion = currentMotion;
            NextMotion = nextMotion;
            ActionId = actionId;
        }
    }
}