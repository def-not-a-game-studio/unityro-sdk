using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityRO.Net.Editor {
    [InitializeOnLoad]
    public class NetworkSnifferWindow : EditorWindow {
        private List<KeyValuePair<NetworkPacket, bool>> _networkPackets = new();
        private Vector2 _currentScrollPos;

        [MenuItem("Window/NetworkSniffer")]
        public static void ShowWindow() {
            EditorWindow.GetWindow(typeof(NetworkSnifferWindow), false, "Network Sniffer");
        }

        private void OnEnable() {
            NetworkClient.OnPacketEvent += OnPacketReceived;
        }

        private void OnDisable() {
            NetworkClient.OnPacketEvent -= OnPacketReceived;
        }

        private void OnGUI() {
            _currentScrollPos = EditorGUILayout.BeginScrollView(_currentScrollPos, GUILayout.Width(300));
            foreach (var keypair in _networkPackets) {
                if (keypair.Key is InPacket In) {
                    // var posDir = (In is ZC.NOTIFY_MOVEENTRY11 moveentry11) ? moveentry11.entityData.PosDir :
                    //     (In is ZC.NOTIFY_STANDENTRY11 standentry11) ? standentry11.entityData.PosDir :
                    //     (In is ZC.NOTIFY_NEWENTRY11 newentry11) ? newentry11.entityData.PosDir : null;
                    //
                    // var str = "";
                    // if (posDir != null) {
                    //     str = string.Join("", posDir);
                    // }

                    GUILayout.Label($"<< {In.Header}");
                } else if (keypair.Key is OutPacket Out) {
                    GUILayout.Label($">> {Out.Header}");
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void OnInspectorUpdate() {
            Repaint();
        }

        private void OnPacketReceived(NetworkPacket packet, bool isHandled) {
            _networkPackets.Add(new KeyValuePair<NetworkPacket, bool>(packet, isHandled));
        }
    }
}