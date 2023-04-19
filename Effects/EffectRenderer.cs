using System;
using UnityEngine;

namespace UnityRO.core.Effects {
    
    public class EffectRenderer : MonoBehaviour {

        [SerializeField] private Effect Effect;
        [SerializeField] private bool autoStart = false;

        private void Start() {
            if (autoStart)
                InitEffects();
        }

        public void InitEffects() {
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
                        param.delay = j * param.timeBetweenDuplication;
                    }
                }
            }

            if (Effect.ThreeDParts.Length > 0) {
                for (int i = 0; i < Effect.ThreeDParts.Length; i++) {
                    var param = Effect.ThreeDParts[i];

                    var threeDRenderer = new GameObject($"3D{i}").AddComponent<ThreeDEffectRenderer>();
                    threeDRenderer.transform.SetParent(transform, false);
                    
                    var time = GameManager.Tick;
                    var instanceParam = new EffectInstanceParam {
                        position = transform.position,
                        otherPosition = transform.position + Vector3.left * 5,
                        startTick = time,
                        endTick = time + param.duration
                    };

                    var initParam = new EffectInitParam {
                        ownerAID = 0
                    };

                    threeDRenderer.Init(param, instanceParam, initParam);
                }
            }
        }
    }
}