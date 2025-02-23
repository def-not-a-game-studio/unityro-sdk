using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityRO.Core.Effects;
using UnityRO.Core.GameEntity;
using UnityRO.Net;

namespace UnityRO.Core {
    public class EntityManager : ManagedMonoBehaviour {
        [SerializeField] private Transform EntitiesParent;
        [SerializeField] private CoreGameEntity PCPrefab;
        [SerializeField] private CoreGameEntity MobPrefab;
        [SerializeField] private FloatingText FloatingTextPrefab;

        private ObjectPool<CoreGameEntity> PCPool;
        private ObjectPool<CoreGameEntity> MobPool;
        private ObjectPool<FloatingText> FloatingTextPool;

        private Dictionary<uint, CoreGameEntity> entityCache = new();

        private NetworkClient NetworkClient;
        private SessionManager SessionManager;

        private void Awake() {
            DontDestroyOnLoad(this);

            NetworkClient = FindObjectOfType<NetworkClient>();
            SessionManager = FindObjectOfType<SessionManager>();

            NetworkClient.HookPacket<ZC.NOTIFY_NEWENTRY11>(ZC.NOTIFY_NEWENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.HookPacket<ZC.NOTIFY_STANDENTRY11>(ZC.NOTIFY_STANDENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.HookPacket<ZC.NOTIFY_MOVEENTRY11>(ZC.NOTIFY_MOVEENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.HookPacket<ZC.NOTIFY_VANISH>(ZC.NOTIFY_VANISH.HEADER, OnEntityVanish);
            NetworkClient.HookPacket<ZC.NOTIFY_ACT>(ZC.NOTIFY_ACT.HEADER, OnEntityAct);
            NetworkClient.HookPacket<ZC.NOTIFY_ACT3>(ZC.NOTIFY_ACT3.HEADER, OnEntityAct3);
            NetworkClient.HookPacket<ZC.EMOTION>(ZC.EMOTION.HEADER, OnEntityEmotion);
            NetworkClient.HookPacket<ZC.SPRITE_CHANGE2>(ZC.SPRITE_CHANGE2.HEADER, OnSpriteChange);
            NetworkClient.HookPacket<ZC.NPCSPRITE_CHANGE>(ZC.NPCSPRITE_CHANGE.HEADER, OnNpcSpriteChange);
            NetworkClient.HookPacket<ZC.NOTIFY_EFFECT2>(ZC.NOTIFY_EFFECT2.HEADER, OnEffect2);
            NetworkClient.HookPacket<ZC.NOTIFY_EFFECT>(ZC.NOTIFY_EFFECT.HEADER, OnEffect);
        }

        private void Start() {
            PCPool = CreateDefaultObjectPool(PCPrefab, EntitiesParent, 200);
            MobPool = CreateDefaultObjectPool(MobPrefab, EntitiesParent, 200);
            FloatingTextPool = CreateDefaultObjectPool(FloatingTextPrefab, null, 200);
        }

        private void OnDestroy() {
            NetworkClient.UnhookPacket<ZC.NOTIFY_NEWENTRY11>(ZC.NOTIFY_NEWENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.UnhookPacket<ZC.NOTIFY_STANDENTRY11>(ZC.NOTIFY_STANDENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.UnhookPacket<ZC.NOTIFY_MOVEENTRY11>(ZC.NOTIFY_MOVEENTRY11.HEADER, OnEntitySpawned);
            NetworkClient.UnhookPacket<ZC.NOTIFY_VANISH>(ZC.NOTIFY_VANISH.HEADER, OnEntityVanish);
            NetworkClient.UnhookPacket<ZC.NOTIFY_ACT>(ZC.NOTIFY_ACT.HEADER, OnEntityAct);
            NetworkClient.UnhookPacket<ZC.NOTIFY_ACT3>(ZC.NOTIFY_ACT3.HEADER, OnEntityAct3);
            NetworkClient.UnhookPacket<ZC.EMOTION>(ZC.EMOTION.HEADER, OnEntityEmotion);
            NetworkClient.UnhookPacket<ZC.SPRITE_CHANGE2>(ZC.SPRITE_CHANGE2.HEADER, OnSpriteChange);
            NetworkClient.UnhookPacket<ZC.NPCSPRITE_CHANGE>(ZC.NPCSPRITE_CHANGE.HEADER, OnNpcSpriteChange);
            NetworkClient.UnhookPacket<ZC.NOTIFY_EFFECT2>(ZC.NOTIFY_EFFECT2.HEADER, OnEffect2);
            NetworkClient.UnhookPacket<ZC.NOTIFY_EFFECT>(ZC.NOTIFY_EFFECT.HEADER, OnEffect);
        }

        public CoreGameEntity Spawn(EntitySpawnData data, bool forceNorthDirection) {
            var hasFound = entityCache.TryGetValue(data.AID, out var entity);

            if (!hasFound) {
                entity = data.objecttype.GetEntityType() == EntityType.PC ? PCPool.Get() : MobPool.Get();
                entity.gameObject.name = data.name;
                entity.Spawn(GetBaseStatus(data), data.PosDir, forceNorthDirection);

                entityCache.Add(data.AID, entity);
            } else {
                entity.UpdateStatus(GetBaseStatus(data), data.PosDir, forceNorthDirection);
            }

            return entity;
        }

        public CoreGameEntity GetEntity(uint AID) {
            var hasFound = entityCache.TryGetValue(AID, out var entity);
            if (hasFound) {
                entity.gameObject.SetActive(true);
                return entity;
            } else if (AID == SessionManager.CurrentSession.AccountID ||
                       AID == SessionManager.CurrentSession.Entity.GetEntityGID()) {
                return SessionManager.CurrentSession.Entity as CoreGameEntity;
            } else {
                //Debug.LogError($"No Entity found for given ID: {AID}");
                return null;
            }
        }

        public void ClearEntities() {
            entityCache.Values.ToList().ForEach(it => {
                if (it.Status.EntityType == EntityType.PC) {
                    PCPool.Release(it);
                } else {
                    MobPool.Release(it);
                }
            });
            entityCache.Clear();
        }

        public void HideEntity(uint AID) {
            if (!entityCache.TryGetValue(AID, out var entity)) return;
            if (entity.Status.EntityType == EntityType.PC) {
                PCPool.Release(entity);
            } else {
                MobPool.Release(entity);
            }
        }

        public void DestroyEntity(uint AID) {
            if (!entityCache.TryGetValue(AID, out var entity)) return;
            if (entity.Status.EntityType == EntityType.PC) {
                PCPool.Release(entity);
            } else {
                MobPool.Release(entity);
            }

            entityCache.Remove(AID);
        }

        public void RecycleEntity(CoreGameEntity entity) {
            if (entity.Status.EntityType == EntityType.PC) {
                PCPool.Release(entity);
            } else {
                MobPool.Release(entity);
            }
        }

        public void UnlinkEntity(uint AID) {
            if (entityCache.ContainsKey(AID)) {
                entityCache.Remove(AID);
            }
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
            var destination = actionRequest.action is not (ActionRequestType.SIT or ActionRequestType.STAND)
                ? GetEntity(actionRequest.targetAID)
                : null;
            var target = actionRequest.damage > 0 ? destination : source;

            if (actionRequest.AID == SessionManager.CurrentSession.AccountID ||
                actionRequest.AID == SessionManager.CurrentSession.Entity.GetEntityGID()) {
                source = SessionManager.CurrentSession.Entity as CoreGameEntity;
            } else if (actionRequest.targetAID == SessionManager.CurrentSession.AccountID ||
                       actionRequest.targetAID == SessionManager.CurrentSession.Entity.GetEntityGID()) {
                destination = SessionManager.CurrentSession.Entity as CoreGameEntity;
            }

            source.SetAttackSpeed(actionRequest.sourceSpeed);
            source.SetAction(actionRequest, true);

            if (actionRequest.IsAttackAction() && destination != null) {
                FloatingTextPool.Get(out var floatingText);
                floatingText.transform.SetParent(target.transform, false);
                switch (actionRequest.action) {
                    case ActionRequestType.ATTACK_MULTIPLE_NOMOTION:
                    case ActionRequestType.ATTACK:
                        floatingText.SetText($"{actionRequest.damage}", Color.white, FloatingTextPool.Release);
                        //target.Damage(pkt.damage, GameManager.Tick + pkt.sourceSpeed);
                        break;

                    // double attack
                    case ActionRequestType.ATTACK_MULTIPLE:
                        floatingText.SetText($"{actionRequest.damage}", Color.white, FloatingTextPool.Release);
                        // Display combo only if entity is mob and the attack don't miss
                        // if (dstEntity.Type == EntityType.MOB && pkt.damage > 0) {
                        //     dstEntity.Damage(pkt.damage / 2, GameManager.Tick + pkt.sourceSpeed * 1, DamageType.COMBO);
                        //     dstEntity.Damage(pkt.damage, GameManager.Tick + pkt.sourceSpeed * 2, DamageType.COMBO | DamageType.COMBO_FINAL);
                        // }

                        // target.Damage(pkt.damage / 2, GameManager.Tick + pkt.sourceSpeed * 1);
                        // target.Damage(pkt.damage / 2, GameManager.Tick + pkt.sourceSpeed * 2);
                        break;

                    // TODO: critical damage
                    case ActionRequestType.ATTACK_CRITICAL:
                        floatingText.SetText($"{actionRequest.damage}", Color.white, FloatingTextPool.Release);
                        // target.Damage(pkt.damage, GameManager.Tick + pkt.sourceSpeed);
                        break;

                    // TODO: lucky miss
                    case ActionRequestType.ATTACK_LUCKY:
                        floatingText.SetText($"{actionRequest.damage}", Color.white, FloatingTextPool.Release);
                        // target.Damage(0, GameManager.Tick + pkt.sourceSpeed);
                        break;
                }

                source.LookTo(destination.gameObject.transform.position);

                // using the source to delay the movement of the target seems wrong
                // var delay = GameManager.Tick + (long)source.GetActionDelay(actionRequest);
                destination.SetAttackedSpeed(actionRequest.targetSpeed);
                // var delay = (long)source.GetActionDelay(actionRequest);
                destination.SetAction(actionRequest, false);
            }
        }

        private void OnEntityEmotion(ushort cmd, int size, ZC.EMOTION packet) {
            var entity = GetEntity(packet.GID);
            if (entity == null) {
                return;
            }

            entity.ShowEmotion(packet.type);
        }

        private void OnSpriteChange(ushort cmd, int size, ZC.SPRITE_CHANGE2 packet) {
            var entity = GetEntity(packet.GID);
            if (entity == null) {
                return;
            }

            entity.ChangeLook((LookType)packet.type, packet.value, packet.value2);
        }

        private void OnNpcSpriteChange(ushort cmd, int size, ZC.NPCSPRITE_CHANGE packet) {
            var entity = GetEntity(packet.GID);
            if (entity == null) {
                return;
            }

            entity.ChangeLook(LookType.LOOK_BASE, (short)packet.value, 0);
        }
        
        private void OnEffect2(ushort cmd, int size, ZC.NOTIFY_EFFECT2 packet)
        {
            var entity = GetEntity(packet.GID);
            entity.ShowEffect(packet.EffectId);
        }
        
        private void OnEffect(ushort cmd, int size, ZC.NOTIFY_EFFECT packet)
        {
            var entity = GetEntity((uint)packet.GID);
            entity.ShowEffect(packet.EffectId);
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

        private ObjectPool<T> CreateDefaultObjectPool<T>(T prefab, Transform parent, int defaultCapacity) where T : MonoBehaviour {
            return new ObjectPool<T>(
                createFunc: () => parent != null ? Instantiate(prefab, parent) : Instantiate(prefab),
                actionOnGet: (it) => it.gameObject.SetActive(true),
                actionOnRelease: (it) => it.gameObject.SetActive(false),
                actionOnDestroy: Destroy,
                collectionCheck: true,
                defaultCapacity: defaultCapacity
            );
        }
    }
}