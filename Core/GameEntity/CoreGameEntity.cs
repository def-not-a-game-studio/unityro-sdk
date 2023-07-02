using UnityEngine;

namespace UnityRO.Core.GameEntity {
    public abstract class CoreGameEntity : ManagedMonoBehaviour, INetworkEntity {
        public abstract GameEntityBaseStatus Status { get; }
        public abstract EntityState State { get; }

        public abstract void ChangeMotion(MotionRequest request);
        public abstract void ChangeDirection(Direction direction);
        public abstract void LookTo(Vector3 position);

        public abstract void Init(GameEntityBaseStatus gameEntityBaseStatus);
        public abstract void Spawn(GameEntityBaseStatus status, int[] PosDir, bool forceNorthDirection);

        public abstract bool HasAuthority();
        public int GetEntityType() => (int)Status.EntityType;
        public int GetEntityAID() => Status.AID;
        public string GetEntityName() => Status.Name;

        public abstract int GetEntityGID();

        /// <summary>
        /// Use this when you have an offset to add to the current position to walk to
        /// </summary>
        /// <param name="destination"></param>
        public abstract void RequestOffsetMovement(Vector2 destination);

        /// <summary>
        /// Use this when you have an absolute coordinate to walk to
        /// </summary>
        /// <param name="destination"></param>
        public abstract void RequestMovement(Vector2 destination);

        public abstract void Vanish(VanishType vanishType);
        public abstract void SetAction(EntityActionRequest actionRequest, bool isSource, long delay = 0);
        public abstract float GetActionDelay(EntityActionRequest actionRequest);
        public abstract void SetAttackSpeed(ushort attackSpeed);
        public abstract void ShowEmotion(byte emotionType);
        public abstract void ChangeLook(LookType lookType, short packetValue, short packetValue2);
        public abstract void UpdateStatus(GameEntityBaseStatus status);
        public abstract void RequestAction(CoreGameEntity target);
        public abstract void TalkToNpc(CoreSpriteGameEntity target);
        public abstract void SetAttackedSpeed(ushort attackedSpeed);
        public abstract void SetState(EntityState state);
    }
}