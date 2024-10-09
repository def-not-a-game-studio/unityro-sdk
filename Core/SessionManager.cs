using Core.Scene;
using UnityRO.Net;

namespace UnityRO.Net {
    using System;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;
    using UnityRO.Core;

    public class SessionManager : MonoBehaviour
    {

        public static UnityAction<GameMap> OnSessionMapChanged;
        
        public Session CurrentSession { get; private set; }
        private Scene CurrentScene;

        private NetworkClient NetworkClient;
        private EntityManager EntityManager;

        private void Awake() {
            NetworkClient = FindObjectOfType<NetworkClient>();
            EntityManager = FindObjectOfType<EntityManager>();
        }

        private void Start() {
            NetworkClient.HookPacket<ZC.NPCACK_MAPMOVE>(ZC.NPCACK_MAPMOVE.HEADER, OnEntityMoved);
            //NetworkClient.HookPacket<ZC.PAR_CHANGE>(ZC.NPCACK_MAPMOVE.HEADER, OnParamChanged);
        }

        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void StartSession(INetworkEntity networkEntity, int accountId) {
            CurrentSession = new Session(networkEntity, accountId);
        }

        public async Task SetCurrentMap(string mapName) {
            if (CurrentSession.CurrentMap == mapName) {
                return;
            }

            var mapScene = SceneManager.GetSceneByName(CurrentSession.CurrentMap ?? "");
            if (mapScene.isLoaded) {
                await GameSceneManager.UnloadScene(CurrentScene.name);
            }
            // else if (!mapScene.IsValid())
            // {
            //     NetworkClient.ResumePacketHandling();
            //     new CZ.NOTIFY_ACTORINIT().Send();
            //     new CZ.REQUEST_CHAT(CurrentSession.Entity.GetEntityName(), "@warp prontera").Send();
            //     return;
            // }

            CurrentSession.SetCurrentMap(mapName);
            var gameMap = FindObjectOfType<GameMap>();
            if (gameMap != null) {
                OnSessionMapChanged?.Invoke(gameMap);
            }

            try {
                NetworkClient.PausePacketHandling();
                await GameSceneManager.LoadScene(CurrentSession.CurrentMap, LoadSceneMode.Additive);
            } catch (Exception e) {
                Debug.LogError($"Map not found on build index: {e.Message}");
            } finally {
                NetworkClient.ResumePacketHandling();
            }
        }

        private void OnSceneLoaded(Scene currentScene, LoadSceneMode arg1) {
            CurrentScene = currentScene;
        }

        #region Session Entity Packets
        private async void OnEntityMoved(ushort cmd, int size, ZC.NPCACK_MAPMOVE pkt) {
            if (CurrentSession.CurrentMap != pkt.MapName) {
                EntityManager.ClearEntities();
                await SetCurrentMap(pkt.MapName);
            }
        }
        #endregion
    }
}