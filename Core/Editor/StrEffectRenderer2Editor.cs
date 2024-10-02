using Core.Effects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StrEffectRenderer))]
internal class StrEffectRenderer2Editor : Editor {
    public override void OnInspectorGUI() {
        var component = (StrEffectRenderer)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("Replay Effect")) {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            component.Replay();
        }
    }
}