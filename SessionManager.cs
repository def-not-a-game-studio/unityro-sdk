using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityRO.Net {
    public class SessionManager : MonoBehaviour {
        public Session CurrentSession { get; private set; }
        private Scene CurrentScene;

        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void StartSession(NetworkEntity networkEntity, int accountId) {
            CurrentSession = new Session(networkEntity, accountId);
        }

        public async Task SetCurrentMap(string mapName) {
            var mapScene = SceneManager.GetSceneByName(CurrentSession.CurrentMap ?? "");
            if (mapScene.isLoaded) {
                await UnloadScene(CurrentScene.name);
            }

            CurrentSession.SetCurrentMap(mapName);
            await LoadScene(CurrentSession.CurrentMap, LoadSceneMode.Additive);
        }

        private void OnSceneLoaded(Scene currentScene, LoadSceneMode arg1) {
            CurrentScene = currentScene;
        }

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
    }
}