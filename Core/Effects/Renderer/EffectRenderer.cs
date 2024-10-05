using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using _3rdparty.unityro_sdk.Core.Effects;
using Core.Effects.EffectParts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityRO.Core;
using UnityRO.Core.Sprite;

namespace Core.Effects
{
    public abstract class EffectRendererPart
    {
        public UnityAction<EffectRendererPart> OnEnd;
        public UnityAction<AudioClip> OnAudio;

        public abstract void Update(Matrix4x4 matrix);
    }

    public class EffectRenderer : ManagedMonoBehaviour
    {
        private EffectCache _effectCache;
        private AudioSource _audioSource;

        private List<EffectRendererPart> Parts = new();
        private List<EffectRendererPart> CopyParts;

        private void Start()
        {
            _effectCache = FindAnyObjectByType<EffectCache>();
            _audioSource = FindAnyObjectByType<AudioSource>();
        }

        public void InitEffects(Effect effect)
        {
            if (effect.CylinderParts.Length > 0) InitCylinder(effect);
            if (effect.ThreeDParts.Length > 0) Init3D(effect);
            if (effect.STRParts.Length > 0) InitStr(effect).Forget();
            if (effect.SPRParts.Length > 0) InitSpr(effect);
        }

        private void InitCylinder(Effect effect)
        {
            for (int i = 0; i < effect.CylinderParts.Length; i++)
            {
                var param = effect.CylinderParts[i];

                var cylinderRenderer = new GameObject($"Cylinder{i}").AddComponent<CylinderEffectRenderer>();
                cylinderRenderer.gameObject.layer = LayerMask.NameToLayer("Effects");
                cylinderRenderer.transform.SetParent(transform, false);
                cylinderRenderer.SetPart(param, param.delay);

                for (int j = 1; j <= param.duplicates; j++)
                {
                    var cylinderJ = new GameObject($"Cylinder{i}-{j}").AddComponent<CylinderEffectRenderer>();
                    cylinderJ.gameObject.layer = LayerMask.NameToLayer("Effects");
                    cylinderJ.transform.SetParent(transform, false);
                    cylinderJ.SetPart(param, j * param.timeBetweenDuplication);
                }
            }
        }

        private void Init3D(Effect effect)
        {
            for (int i = 0; i < effect.ThreeDParts.Length; i++)
            {
                var param = effect.ThreeDParts[i];

                var threeDRenderer = new GameObject($"3D{i}").AddComponent<ThreeDEffectRenderer>();
                threeDRenderer.gameObject.layer = LayerMask.NameToLayer("Effects");
                //threeDRenderer.gameObject.GetOrAddComponent<Billboard>();
                threeDRenderer.transform.SetParent(transform, false);

                var time = GameManager.Tick;
                var instanceParam = new EffectInstanceParam
                {
                    position = transform.position,
                    otherPosition = transform.position + Vector3.left * 5,
                    startTick = time,
                    endTick = time + param.duration
                };

                var initParam = new EffectInitParam
                {
                    ownerAID = 0
                };

                threeDRenderer.Init(param, instanceParam, initParam);
            }
        }

        private async UniTask InitStr(Effect effect)
        {
            foreach (var param in effect.STRParts)
            {
                var renderer = new StrEffectRenderer();
                var renderInfo = await _effectCache.GetRenderInfo(effect.EffectId, param);
                renderer.OnAudio += PlayPartAudioClip;
                renderer.OnEnd += OnPartEnd;
                renderer.Initialize(renderInfo);
                Parts.Add(renderer);
            }
        }

        private void InitSpr(Effect effect)
        {
            var entity = gameObject.AddComponent<EffectGameEntity>();
            foreach (var param in effect.SPRParts)
            {
                var renderer = new SpriteEffectRenderer();
                renderer.OnAudio += PlayPartAudioClip;
                renderer.OnEnd += OnPartEnd;
                renderer.Init(param.file, param.wav, ViewerType.Effect, entity);
                Parts.Add(renderer);
            }
        }

        private void OnPartEnd(EffectRendererPart part)
        {
            Parts.Remove(part);
        }

        private void PlayPartAudioClip(AudioClip clip)
        {
            _audioSource.clip = clip;
            _audioSource.Play();
        }

        public void Vanish()
        {
            Destroy(gameObject);
        }

        public override void ManagedUpdate()
        {
            if (Parts.Count == 0)
                return;
            
            lock (Parts) {
                CopyParts = new List<EffectRendererPart>(Parts);
            }

            foreach (var part in CopyParts)
            {
                part?.Update(transform.localToWorldMatrix);
            }
        }

        public void SetEffect(Effect effect)
        {
            InitEffects(effect);
        }
    }
}