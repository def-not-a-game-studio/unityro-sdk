using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityRO.Net.Editor {
    public class ReplayHelperWindow : EditorWindow {
        private NetworkClient NetworkClient;

        [SerializeField] private RecordedNetworkTraffic ReplayFile;
        [SerializeField] private bool RecordFromStart;
        [SerializeField] private bool FileName;

        private int _currentPacket;
        private int _maxPackets;
        private bool isReplayLoaded = false;

        [MenuItem("Window/Replay Helper")]
        public static void ShowWindow() {
            GetWindow(typeof(ReplayHelperWindow), false, "Replay Helper");
        }

        private void OnEnable() {
            NetworkClient = SceneManager.GetActiveScene()
                .GetRootGameObjects()
                .ToList()
                .Find(it => it.TryGetComponent<NetworkClient>(out var component))
                .GetComponent<NetworkClient>();
            
        }

        private void OnGUI() {
            GUILayout.Space(8);
            GUILayout.Label("Replay Settings", EditorStyles.boldLabel);
            GUILayout.Space(8);

            #region Replay File

            GUILayout.BeginHorizontal();

            ReplayFile = (RecordedNetworkTraffic)EditorGUILayout.ObjectField("Replay File", ReplayFile, typeof(RecordedNetworkTraffic), true);

            if (GUILayout.Button("Load Replay")) {
                if (NetworkClient == null) return;
                NetworkClient.StartReplay(ReplayFile);
            }

            GUILayout.EndHorizontal();

            #endregion

            #region Replay Controls

            GUILayout.Space(8);
            GUILayout.Label("Replay Controls", EditorStyles.boldLabel);
            GUILayout.Space(4);
            GUILayout.Label(isReplayLoaded ? $"Progress: {_currentPacket + 1}/{_maxPackets}" : "Progress: Load replay file to control progress");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous Packet", EditorStyles.miniButtonLeft)) {
                if (NetworkClient == null) return;
                NetworkClient.PreviousReplayPacket();
            }

            if (GUILayout.Button("Play/Pause", EditorStyles.miniButtonMid)) {
                if (NetworkClient == null) return;
                NetworkClient.IsReplayStepping = !NetworkClient.IsReplayStepping;
            }

            if (GUILayout.Button("Next Packet", EditorStyles.miniButtonRight)) {
                if (NetworkClient == null) return;
                NetworkClient.NextReplayPacket();
            }

            GUILayout.EndHorizontal();

            #endregion
        }

        private void OnInspectorUpdate() {
            Repaint();
        }
    }
}