using UnityEngine;
using UnityRO.Core.GameEntity;

namespace _3rdparty.unityro_sdk.Core.Effects
{
    public class EffectGameEntity : CoreSpriteGameEntity
    {
        public override void ManagedUpdate()
        {
        }

        public override GameEntityBaseStatus Status { get; } = new GameEntityBaseStatus()
        {
            EntityType = EntityType.EFFECT
        };

        public override EntityState State { get; } = EntityState.Idle;

        public override void ChangeMotion(MotionRequest request)
        {
        }

        public override void ChangeDirection(Direction direction)
        {
        }

        public override void LookTo(Vector3 position)
        {
        }

        public override void Init(GameEntityBaseStatus gameEntityBaseStatus)
        {
        }

        public override void Spawn(GameEntityBaseStatus status, int[] PosDir, bool forceNorthDirection)
        {
        }

        public override bool HasAuthority() => false;

        public override int GetEntityGID() => -1001;

        public override void RequestOffsetMovement(Vector2 destination)
        {
        }

        public override void RequestMovement(Vector2 destination)
        {
        }

        public override void Vanish(VanishType vanishType)
        {
        }

        public override void SetAction(EntityActionRequest actionRequest, bool isSource, long delay = 0)
        {
        }

        public override float GetActionDelay(EntityActionRequest actionRequest) => 0f;

        public override void SetAttackSpeed(ushort attackSpeed)
        {
        }

        public override void ShowEmotion(byte emotionType)
        {
        }

        public override void ShowEffect(int effectId)
        {
        }

        public override void ChangeLook(LookType lookType, short packetValue, short packetValue2)
        {
        }

        public override void UpdateStatus(GameEntityBaseStatus status, int[] PosDir, bool forceNorthDirection)
        {
        }

        public override void RequestAction(CoreGameEntity target)
        {
        }

        public override void TalkToNpc(CoreGameEntity target)
        {
        }

        public override void SetAttackedSpeed(ushort attackedSpeed)
        {
        }

        public override void SetState(EntityState state)
        {
        }

        public override void DelayMovement(long delay)
        {
        }

        public override Direction Direction { get; set; } = Direction.South;
        public override int HeadDirection { get; } = 0;
        public override GameMap CurrentMap { get; }
        public override float GetDistance() => 0f;
    }
}