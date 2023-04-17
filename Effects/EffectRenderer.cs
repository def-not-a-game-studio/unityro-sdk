using System;
using UnityEngine;

namespace UnityRO.core.Effects {
    
    public class EffectRenderer : MonoBehaviour {

        [SerializeField] private Effect Effect;

        private void Start() {
            if (Effect.CylinderParts.Length > 0) {
                for (int i = 0; i < Effect.CylinderParts.Length; i++) {
                    var param = Effect.CylinderParts[i];
                    
                    var cylinderRenderer = new GameObject($"Cylinder{i}").AddComponent<CylinderEffectRenderer>();
                    cylinderRenderer.transform.SetParent(transform, false);
                    cylinderRenderer.Part = param;

                    for (int j = 1; j <= param.duplicates; j++) {
                        var cylinderJ = new GameObject($"Cylinder{i}-{j}").AddComponent<CylinderEffectRenderer>();
                        cylinderJ.transform.SetParent(transform, false);
                        cylinderJ.Part = param;
                        cylinderJ.DelayToStart = j * param.timeBetweenDuplication;
                    }
                }
            }
        }
    }
}