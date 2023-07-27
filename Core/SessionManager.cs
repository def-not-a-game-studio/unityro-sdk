using System;
using System.Threading.Tasks;
using Core.Path;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityRO.Core;

namespace UnityRO.Net {
    public class SessionManager : MonoBehaviour {
        public Session CurrentSession { get; private set; }
        private Scene CurrentScene;

        private NetworkClient NetworkClient;
        private PathFinder PathFinder;
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
                await UnloadScene(CurrentScene.name);
            }

            CurrentSession.SetCurrentMap(mapName);

            try {
                NetworkClient.PausePacketHandling();
                await LoadScene(CurrentSession.CurrentMap, LoadSceneMode.Additive);
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
        
        #region Scene Extension
        private Task<bool> LoadScene(string sceneName, LoadSceneMode mode) {
            var t = new TaskCompletionSource<bool>();

            SceneManager.LoadSceneAsync(sceneName, mode).completed += delegate { t.TrySetResult(true); };

            return t.Task;
        }

        private Task<bool> UnloadScene(string sceneName) {
            var t = new TaskCompletionSource<bool>();

            SceneManager.UnloadSceneAsync(sceneName).completed += delegate { t.TrySetResult(true); };

            return t.Task;
        }
        #endregion
    }
}