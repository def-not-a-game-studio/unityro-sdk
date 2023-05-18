using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityRO.Core {
    public class EntityManager : ManagedMonoBehaviour {
        private Dictionary<uint, CoreGameEntity> entityCache = new();

        private NetworkClient NetworkClient;

        private void Awake() {
            NetworkClient = FindObjectOfType<NetworkClient>();

            NetworkClient.HookPacket<ZC.NOTIFY_NEWENTRY11>(ZC.NOTIFY_NEWENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.HookPacket<ZC.NOTIFY_STANDENTRY11>(ZC.NOTIFY_STANDENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.HookPacket<ZC.NOTIFY_MOVEENTRY11>(ZC.NOTIFY_MOVEENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.HookPacket<ZC.NOTIFY_VANISH>(ZC.NOTIFY_VANISH.HEADER, OnEntityDespawned);
        }

        private void OnDestroy() {
            NetworkClient.UnhookPacket<ZC.NOTIFY_NEWENTRY11>(ZC.NOTIFY_NEWENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.UnhookPacket<ZC.NOTIFY_STANDENTRY11>(ZC.NOTIFY_STANDENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.UnhookPacket<ZC.NOTIFY_MOVEENTRY11>(ZC.NOTIFY_MOVEENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.UnhookPacket<ZC.NOTIFY_VANISH>(ZC.NOTIFY_VANISH.HEADER, OnEntityDespawned);
        }

        public CoreGameEntity Spawn(EntitySpawnData data) {
            var hasFound = entityCache.TryGetValue(data.AID, out var entity);
            
            if (!hasFound) {
                entity = (EntityType)data.objecttype switch {
                    EntityType.PC => SpawnPC(data),
                    EntityType.NPC => SpawnNPC(data),
                    EntityType.MOB => SpawnMOB(data),
                    _ => throw new ArgumentOutOfRangeException()
                };
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
                Debug.LogError($"No Entity found for given ID: {AID}");
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

        private void OnEntitySpawned(ushort cmd, int size, ZC.NOTIFY_STANDENTRY11 packet) { }

        private void OnEntitySpawned(ushort cmd, int size, ZC.NOTIFY_NEWENTRY11 packet) { }

        private void OnEntitySpawned(ushort cmd, int size, ZC.NOTIFY_MOVEENTRY11 packet) { }

        private void OnEntityDespawned(ushort cmd, int size, ZC.NOTIFY_VANISH packet) { }

        private CoreGameEntity SpawnPC(EntitySpawnData data) {
            return null;
        }

        private CoreGameEntity SpawnMOB(EntitySpawnData data) {
            return null;
        }

        private CoreGameEntity SpawnNPC(EntitySpawnData data) {
            return null;
        }

        public override void ManagedUpdate() { }
    }
}