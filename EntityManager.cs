using System.Collections.Generic;
using System.Linq;
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
        }

        private void OnDestroy() {
            NetworkClient.UnhookPacket<ZC.NOTIFY_NEWENTRY11>(ZC.NOTIFY_NEWENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.UnhookPacket<ZC.NOTIFY_STANDENTRY11>(ZC.NOTIFY_STANDENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.UnhookPacket<ZC.NOTIFY_MOVEENTRY11>(ZC.NOTIFY_MOVEENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.UnhookPacket<ZC.NOTIFY_VANISH>(ZC.NOTIFY_VANISH.HEADER, OnEntityVanish);
        }

        public CoreGameEntity Spawn(EntitySpawnData data, bool forceNorthDirection) {
            var hasFound = entityCache.TryGetValue(data.AID, out var entity);

            if (!hasFound) {
                entity = Instantiate(EntityPrefab, EntitiesParent);
                entity.gameObject.name = data.name;
                entity.gameObject.SetActive(false);

                entity.Spawn(GetBaseStatus(data), data.PosDir, forceNorthDirection);

                entityCache.Add(data.AID, entity);
            }

            if (entity != null) {
                entity.gameObject.SetActive(true);
            }

            return entity;
        }

        public CoreGameEntity GetEntity(uint AID) {
            var hasFound = entityCache.TryGetValue(AID, out var entity);
            if (hasFound) {
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

        public void RemoveEntity(uint AID) {
            if (!entityCache.TryGetValue(AID, out var entity)) return;
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
            };
        }
    }
}