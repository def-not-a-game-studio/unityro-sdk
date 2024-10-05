using System.Collections.Generic;
using _3rdparty.unityro_sdk.Core.Effects;
using Core.Effects.EffectParts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityRO.Core;
using UnityRO.Core.Sprite;

namespace Core.Effects {
    
    public class EffectRenderer : ManagedMonoBehaviour {

        [SerializeField] public Effect Effect;
        [SerializeField] private bool autoStart = false;

        private EffectCache _effectCache;
        private bool IsInit = false;

        private void Start() {
            _effectCache = FindAnyObjectByType<EffectCache>();
            if (autoStart)
                InitEffects().Forget();
        }

        public UniTask InitEffects() {
            if (Effect.CylinderParts.Length > 0) InitCylinder();
            if (Effect.ThreeDParts.Length > 0) Init3D();
            if (Effect.STRParts.Length > 0) InitStr().Forget();
            if (Effect.SPRParts.Length > 0) InitSpr();

            IsInit = true;
            
            return UniTask.CompletedTask;
        }

        private void InitCylinder()
        {
            for (int i = 0; i < Effect.CylinderParts.Length; i++) {
                var param = Effect.CylinderParts[i];

                var cylinderRenderer = new GameObject($"Cylinder{i}").AddComponent<CylinderEffectRenderer>();
                cylinderRenderer.gameObject.layer = LayerMask.NameToLayer("Effects");
                cylinderRenderer.transform.SetParent(transform, false);
                cylinderRenderer.SetPart(param, param.delay);

                for (int j = 1; j <= param.duplicates; j++) {
                    var cylinderJ = new GameObject($"Cylinder{i}-{j}").AddComponent<CylinderEffectRenderer>();
                    cylinderJ.gameObject.layer = LayerMask.NameToLayer("Effects");
                    cylinderJ.transform.SetParent(transform, false);
                    cylinderJ.SetPart(param, j * param.timeBetweenDuplication);
                }
            }
        }
        
        private void Init3D()
        {
            for (int i = 0; i < Effect.ThreeDParts.Length; i++) {
                var param = Effect.ThreeDParts[i];

                var threeDRenderer = new GameObject($"3D{i}").AddComponent<ThreeDEffectRenderer>();
                threeDRenderer.gameObject.layer = LayerMask.NameToLayer("Effects");
                //threeDRenderer.gameObject.GetOrAddComponent<Billboard>();
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

        private async UniTaskVoid InitStr()
        {
            for (int i = 0; i < Effect.STRParts.Length; i++)
            {
                var param = Effect.STRParts[i];
                
                var renderer = new GameObject($"STR{i}").AddComponent<StrEffectRenderer>();
                renderer.gameObject.layer = LayerMask.NameToLayer("Effects");
                renderer.transform.SetParent(transform, false);

                var renderInfo = await _effectCache.GetRenderInfo(Effect.EffectId, param);
                renderer.Initialize(renderInfo);
            }
        }

        private void InitSpr()
        {
            for (int i = 0; i < Effect.SPRParts.Length; i++)
            {
                var param = Effect.SPRParts[i];
                
                var renderer = new GameObject($"SPR{i}").AddComponent<SpriteEffectRenderer>();
                renderer.gameObject.layer = LayerMask.NameToLayer("Effects");
                renderer.transform.SetParent(transform, false);
                
                renderer.Init(param.file, param.wav, ViewerType.Effect);
            }
        }

        public void Vanish() {
            Destroy(gameObject);
        }

        public override void ManagedUpdate()
        {
            if (Effect is not null && !IsInit)
            {
                InitEffects();
            }
        }

        public void SetEffect(Effect effect)
        {
            Effect = effect;
            InitEffects();
        }
    }
}