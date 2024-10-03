using Core.Effects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LegacyStrEffectRenderer))]
internal class LegacyStrEffectRendererEditor : Editor {
    public override void OnInspectorGUI() {
        var component = (LegacyStrEffectRenderer)target;
        base.OnInspectorGUI();

        if (GUILayout.Button("Replay Effect")) {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            component.Replay();
        }
    }
}