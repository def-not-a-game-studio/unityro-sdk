using ROIO.Models.FileTypes;
using UnityEngine;
using UnityRO.Core.Camera;
using UnityRO.Core.Database;
using UnityRO.Core.GameEntity;

namespace UnityRO.Core.Sprite {
    public class FramePaceCalculator {
        private const int AVERAGE_ATTACK_SPEED = 435;
        private const int AVERAGE_ATTACKED_SPEED = 288;
        private const int MAX_ATTACK_SPEED = AVERAGE_ATTACKED_SPEED * 2;

        private CoreSpriteGameEntity Entity;
        private ISpriteViewer SpriteViewer;
        private CharacterCamera CharacterCamera;

        public SpriteMotion CurrentSpriteMotion { get; private set; }

        public int CurrentFrame { get; private set; } = 0;
        private long FrameStart = GameManager.Tick;
        private float CurrentDelay = 0f;
        private ACT CurrentACT;
        private ACT.Action CurrentAction;
        private int ActionId;
        private int FixedActionIndex = -1;

        private float AttackMotion = 6f;
        private float MotionSpeed = 1f;

        public FramePaceCalculator(CoreSpriteGameEntity entity, ISpriteViewer viewer, ACT currentAct, CharacterCamera characterCamera) {
            Entity = entity;
            SpriteViewer = viewer;
            CurrentACT = currentAct;
            CharacterCamera = characterCamera;
        }
        
        public FramePaceCalculator(CoreSpriteGameEntity entity, ISpriteViewer viewer, ACT currentAct) {
            Entity = entity;
            SpriteViewer = viewer;
            CurrentACT = currentAct;
        }

        public int GetActionIndex() {
            if (FixedActionIndex >= 0) {
                return FixedActionIndex % CurrentACT.actions.Length;
            }

            var cameraDirection = (int)(CharacterCamera?.Direction ?? 0);
            var entityDirection = (int)Entity.Direction + 8;

            return (ActionId + (cameraDirection + entityDirection) % 8) % CurrentACT.actions.Length;
        }

        public ACT.Frame GetCurrentFrame() {
            CurrentAction = CurrentACT.actions[GetActionIndex()];

            var isIdle = (Entity.Status.EntityType == EntityType.PC && CurrentSpriteMotion is SpriteMotion.Idle or SpriteMotion.Sit or SpriteMotion.Dead);
            var frameCount = CurrentAction.frames.Length;
            var deltaSinceMotionStart = (GameManager.Tick - FrameStart);
            var maxFrame = frameCount - 1;

            if (isIdle) {
                CurrentFrame = Entity.HeadDirection;
            }

            CurrentDelay = GetDelay();
            if (deltaSinceMotionStart >= CurrentDelay && CurrentFrame < maxFrame && !isIdle) {
                FrameStart = GameManager.Tick;
                CurrentFrame++;
            }
            
            if (CurrentFrame >= maxFrame) {
                if (AnimationHelper.IsLoopingMotion(CurrentSpriteMotion) && SpriteViewer.GetViewerType() != ViewerType.Emotion) {
                    CurrentFrame = 0;
                } else { //might need to check if it's body to call the animation finished
                    SpriteViewer.OnAnimationFinished();
                    if (CurrentFrame > 0) {
                        CurrentFrame = maxFrame - 1;
                    }
                }
            }
            
            return CurrentAction.frames[CurrentFrame];
        }

        public float GetDelay() {
            CurrentAction ??= CurrentACT.actions[GetActionIndex()];
            
            if (SpriteViewer.GetViewerType() == ViewerType.Body && CurrentSpriteMotion == SpriteMotion.Walk) {
                return CurrentAction.delay / 150 * Entity.Status.MoveSpeed;
            }

            var delayTime = CurrentSpriteMotion switch
            {
                SpriteMotion.Attack1 or SpriteMotion.Attack2 or SpriteMotion.Attack3 => AttackMotion * GetMotionSpeed() / CurrentAction.frames.Length,
                SpriteMotion.Hit => GetMotionSpeed() / CurrentAction.frames.Length,
                _ => CurrentAction.delay
            };
            
            return delayTime;
        }

        // TODO check if there's no need use the action delay * AttackMotion instead 
        // rA found out they need to find the current attacked action index and multiply by the action default delay
        public float GetAttackDelay() {
            ProcessAttackMotion();
            var delayTime = AttackMotion * GetMotionSpeed();
            if (delayTime < 0) {
                delayTime = 0;
            }

            return delayTime;
        }

        public void OnMotionChanged(MotionRequest motionRequest) {
            if (CurrentSpriteMotion is SpriteMotion.Attack1 or SpriteMotion.Attack2 or SpriteMotion.Attack3) {
                ProcessAttackMotion();
            }

            FrameStart = GameManager.Tick;
            CurrentFrame = 0;
            CurrentSpriteMotion = motionRequest.Motion;
            ActionId = AnimationHelper.GetMotionIdForSprite(Entity.Status.EntityType, CurrentSpriteMotion);

            CurrentDelay = GetDelay();
        }

        private int GetAttackMotion() {
            for (var i = 0; i < CurrentAction.frames.Length; i++) {
                var frame = CurrentAction.frames[i];
                if (frame.soundId == -1) continue;
                if (frame.soundId < CurrentACT.sounds.Length && CurrentACT.sounds[frame.soundId] == "atk") {
                    return i;
                }
            }

            var r = CurrentAction.frames.Length - 1;
            if (r > 0)
                r -= 1;

            return r;
        }

        private void ProcessAttackMotion() {
            MotionSpeed = GetMotionSpeed();
            AttackMotion = GetAttackMotion();

            var isSecondAttack = WeaponTypeDatabase.IsSecondAttack(
                Entity.Status.Job,
                Entity.Status.IsMale ? 1 : 0,
                Entity.Status.Weapon,
                Entity.Status.Shield
            );

            if (isSecondAttack) {
                if ((JobType)Entity.Status.Job is JobType.JT_NOVICE or JobType.JT_SUPERNOVICE or JobType.JT_SUPERNOVICE_B) {
                    if (Entity.Status.IsMale) {
                        AttackMotion = 5.85f;
                    }
                } else if ((JobType)Entity.Status.Job is JobType.JT_ASSASSIN or JobType.JT_ASSASSIN_H
                           or JobType.JT_ASSASSIN_B) {
                    switch ((WeaponType)Entity.Status.Weapon) {
                        case WeaponType.CATARRH:
                        case WeaponType.SHORTSWORD_SHORTSWORD:
                        case WeaponType.SWORD_SWORD:
                        case WeaponType.AXE_AXE:
                        case WeaponType.SHORTSWORD_SWORD:
                        case WeaponType.SHORTSWORD_AXE:
                        case WeaponType.SWORD_AXE:
                            AttackMotion = 3.0f;
                            break;
                    }
                }
            } else {
                AttackMotion = (JobType)Entity.Status.Job switch {
                    JobType.JT_THIEF => 5.75f,
                    JobType.JT_MERCHANT => 5.85f,
                    _ => AttackMotion
                };
            }

            var usingArrow = WeaponTypeDatabase.IsWeaponUsingArrow(Entity.Status.Weapon);
            if (usingArrow) {
                //TODO some additional checks see Pc.cpp line 847
                // Dividing by 25f to get rid of the mult we do when reading the act delays...
                AttackMotion += 8 / (MotionSpeed / 25f);
            }
        }

        private float GetMotionSpeed() {
            CurrentAction = CurrentACT.actions[GetActionIndex()];
            var motionSpeed = CurrentSpriteMotion switch {
                SpriteMotion.Hit => Entity.Status.AttackedSpeed,
                SpriteMotion.Attack1 or SpriteMotion.Attack2 or SpriteMotion.Attack3 => Entity.Status.AttackSpeed > MAX_ATTACK_SPEED ? MAX_ATTACK_SPEED : Entity.Status.AttackSpeed,
                _ => 0
            };

            var multiplier = CurrentSpriteMotion switch
            {
                SpriteMotion.Hit => motionSpeed / (float)AVERAGE_ATTACKED_SPEED,
                _ => motionSpeed / (float)AVERAGE_ATTACK_SPEED
            };
            
            // we divide by 25 because when we read from ACT we multiply by 25. IDK why
            var finalSpeed = CurrentAction.delay / 25 * multiplier;
            // then we multiply by 24 because DHXJ mod their tick by 24 
            return finalSpeed * 24;
        }

        private void PCLog(string message) {
            if (Entity.GetEntityType() == (int)EntityType.PC && SpriteViewer.GetViewerType() == ViewerType.Body && !Entity.HasAuthority()) {
                //Debug.Log(message);
            }
        }

        public void SetActionIndex(int actionIndex) {
            FixedActionIndex = actionIndex;
        }
    }
}