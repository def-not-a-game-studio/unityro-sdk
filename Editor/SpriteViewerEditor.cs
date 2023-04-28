using System;
using UnityEditor;
using UnityEngine;

namespace _3rdparty.unityro_core.Editor {
    [CustomEditor(typeof(SpriteViewer))]
    public class EntityViewerEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var component = (SpriteViewer)target;
            base.OnInspectorGUI();
            
            GUILayout.Space(20);
            GUILayout.Label("Force Motion");

            if (GUILayout.Button("Attack > Standby")) {
                component.ChangeMotion(
                    new MotionRequest { Motion = SpriteMotion.Attack },
                    new MotionRequest { Motion = SpriteMotion.Standby }
                );
            }

            if (GUILayout.Button("Hit > Standby")) {
                component.ChangeMotion(
                    new MotionRequest { Motion = SpriteMotion.Hit },
                    new MotionRequest { Motion = SpriteMotion.Standby }
                );
            }

            foreach (var motion in Enum.GetValues(typeof(SpriteMotion))) {
                if (GUILayout.Button(((SpriteMotion)motion).ToString())) {
                    if ((SpriteMotion)motion == SpriteMotion.Attack) {
                        //component.Entity.SetAttackSpeed(380);
                    }
                    component.ChangeMotion(
                        new MotionRequest { Motion = (SpriteMotion)motion }
                        // new MotionRequest { Motion = SpriteMotion.Idle }
                    );
                }
            }
        }
    }
}