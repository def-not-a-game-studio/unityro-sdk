using System.Collections.Generic;
using System.Linq;
using Core.Path;
using UnityEngine;
using UnityRO.Core.GameEntity;

namespace UnityRO.Core {
    public class EntityManager : ManagedMonoBehaviour {
        [SerializeField] private CoreGameEntity EntityPrefab;
        [SerializeField] private Transform EntitiesParent;

        private Dictionary<uint, CoreGameEntity> entityCache = new();

        private NetworkClient NetworkClient;

        private void Awake() {
            NetworkClient = FindObjectOfType<NetworkClient>();

            NetworkClient.HookPacket<ZC.NOTIFY_NEWENTRY11>(ZC.NOTIFY_NEWENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.HookPacket<ZC.NOTIFY_STANDENTRY11>(ZC.NOTIFY_STANDENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.HookPacket<ZC.NOTIFY_MOVEENTRY11>(ZC.NOTIFY_MOVEENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.HookPacket<ZC.NOTIFY_VANISH>(ZC.NOTIFY_VANISH.HEADER, OnEntityVanish);
            NetworkClient.HookPacket<ZC.NOTIFY_ACT>(ZC.NOTIFY_ACT.HEADER, OnEntityAct);
            NetworkClient.HookPacket<ZC.NOTIFY_ACT3>(ZC.NOTIFY_ACT3.HEADER, OnEntityAct3);
        }

        private void OnDestroy() {
            NetworkClient.UnhookPacket<ZC.NOTIFY_NEWENTRY11>(ZC.NOTIFY_NEWENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.UnhookPacket<ZC.NOTIFY_STANDENTRY11>(ZC.NOTIFY_STANDENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.UnhookPacket<ZC.NOTIFY_MOVEENTRY11>(ZC.NOTIFY_MOVEENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.UnhookPacket<ZC.NOTIFY_VANISH>(ZC.NOTIFY_VANISH.HEADER, OnEntityVanish);
            NetworkClient.UnhookPacket<ZC.NOTIFY_ACT>(ZC.NOTIFY_ACT.HEADER, OnEntityAct);
            NetworkClient.UnhookPacket<ZC.NOTIFY_ACT3>(ZC.NOTIFY_ACT3.HEADER, OnEntityAct3);
        }

        public CoreGameEntity Spawn(EntitySpawnData data, bool forceNorthDirection) {
            var hasFound = entityCache.TryGetValue(data.AID, out var entity);

            if (!hasFound) {
                entity = Instantiate(EntityPrefab, EntitiesParent);
                entity.gameObject.name = data.name;
                entity.Spawn(GetBaseStatus(data), data.PosDir, forceNorthDirection);

                entityCache.Add(data.AID, entity);
            } else {
                entity.gameObject.SetActive(true);
            }

            return entity;
        }

        public CoreGameEntity GetEntity(uint AID) {
            var hasFound = entityCache.TryGetValue(AID, out var entity);
            if (hasFound) {
                entity.gameObject.SetActive(true);
                return entity;
            } else {
                //Debug.LogError($"No Entity found for given ID: {AID}");
                return null;
            }
        }

        public void ClearEntities() {
            entityCache.Values.ToList().ForEach(Destroy);
            entityCache.Clear();
        }

        public void HideEntity(uint AID) {
            if (!entityCache.TryGetValue(AID, out var entity)) return;
            entity.gameObject.SetActive(false);
        }

        public void DestroyEntity(uint AID) {
            if (!entityCache.TryGetValue(AID, out var entity)) return;
            entity.gameObject.SetActive(false);
            Destroy(entity.gameObject);
            entityCache.Remove(AID);
        }

        private void OnEntitySpawned(ushort cmd, int size, ZC.NOTIFY_STANDENTRY11 packet) {
            Spawn(packet.entityData, true);
        }

        private void OnEntitySpawned(ushort cmd, int size, ZC.NOTIFY_NEWENTRY11 packet) {
            Spawn(packet.entityData, false);
        }

        private void OnEntitySpawned(ushort cmd, int size, ZC.NOTIFY_MOVEENTRY11 packet) {
            Spawn(packet.entityData, false);
        }

        private void OnEntityVanish(ushort cmd, int size, ZC.NOTIFY_VANISH packet) {
            GetEntity(packet.AID)?.Vanish((VanishType)packet.Type);
        }

        private void OnEntityAct3(ushort cmd, int size, ZC.NOTIFY_ACT3 packet) {
            OnEntityAction(packet.ActionRequest);
        }

        private void OnEntityAct(ushort cmd, int size, ZC.NOTIFY_ACT packet) {
            OnEntityAction(packet.ActionRequest);
        }

        private void OnEntityAction(EntityActionRequest actionRequest) {
            var source = GetEntity(actionRequest.AID);
            var target = GetEntity(actionRequest.targetAID);

            if (actionRequest.IsAttackAction() && target != null) {
                source.ChangeDirection(PathFinder.GetDirectionForOffset(target.gameObject.transform.position, source.gameObject.transform.position));
            }

            source.SetAction(actionRequest.action);
            source.SetAttackSpeed(actionRequest.sourceSpeed);
        }

        public override void ManagedUpdate() { }

        private GameEntityBaseStatus GetBaseStatus(EntitySpawnData data) {
            return new GameEntityBaseStatus {
                EntityType = data.objecttype.GetEntityType(),
                GID = (int)data.GID,
                AID = (int)data.AID,
                GUID = (int)data.GuildID,
                Name = data.name,
                Job = data.job,
                IsMale = data.sex == 1,

                HairStyle = data.head,
                HairColor = data.HairColor,
                ClothesColor = data.ClothesColor,

                MoveSpeed = data.speed,

                Weapon = (int)data.Weapon,
                Shield = (int)data.Shield
            };
        }
    }
}