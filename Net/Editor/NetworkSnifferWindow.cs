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
            _currentScrollPos = EditorGUILayout.BeginScrollView(_currentScrollPos, GUILayout.Width(200));
            var defaultColor = GUI.contentColor;
            foreach (var keypair in _networkPackets) {
                GUI.contentColor = keypair.Value ? defaultColor : Color.yellow;
                if (keypair.Key is InPacket In) {
                    if (GUILayout.Button($"<< {In.Header}")) {
                        Debug.Log(In);
                    }
                } else if (keypair.Key is OutPacket Out) {
                    if (GUILayout.Button($">> {Out.Header}")) {
                        Debug.Log(Out);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void OnInspectorUpdate() {
            Repaint();
        }

        private void OnPacketReceived(NetworkPacket packet, bool isHandled) {
            if (_networkPackets.Count > 300) {
                _networkPackets.RemoveAt(0);
            }

            if (packet is InPacket In && In.Header != PacketHeader.ZC_NOTIFY_TIME) {
                _networkPackets.Add(new KeyValuePair<NetworkPacket, bool>(packet, isHandled));
            } else if (packet is OutPacket Out && Out.Header != PacketHeader.CZ_REQUEST_TIME2) {
                _networkPackets.Add(new KeyValuePair<NetworkPacket, bool>(packet, isHandled));
            }
        }
    }
}