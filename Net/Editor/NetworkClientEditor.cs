using UnityEditor;
using UnityEngine;

namespace UnityRO.Net.Editor {
    [CustomEditor(typeof(NetworkClient))]
    public class NetworkClientEditor : UnityEditor.Editor {
        string fileName = "";

        public override void OnInspectorGUI() {
            var component = (NetworkClient)target;
            base.OnInspectorGUI();

            GUILayout.Space(24);
            GUILayout.Label("Record Packets", new GUIStyle() { fontStyle = FontStyle.Bold });

            fileName = EditorGUILayout.TextField("File name", fileName);
            GUILayout.Space(8);
            if (component.IsRecording) {
                if (GUILayout.Button("Save recording")) {
                    if (fileName.Length > 0) {
                        component.StopRecording(fileName);
                    } else {
                        Debug.LogError("Replay file name must not be empty");
                    }
                }
            } else {
                if (GUILayout.Button("Start recording")) {
                    component.StartRecording();
                }
            }

            if (component.ReplayFile != null) {
                if (GUILayout.Button("Restart replay")) {
                    component.StartReplay();
                }
            }
        }
    }
}