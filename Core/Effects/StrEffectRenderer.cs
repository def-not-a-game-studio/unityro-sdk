using System;
using _3rdparty.unityro_sdk.Core.Effects;
using UnityEngine;
using UnityEngine.Events;
using UnityRO.Core;

namespace Core.Effects
{
    [RequireComponent(typeof(AudioSource))]
    public class StrEffectRenderer : ManagedMonoBehaviour
    {
        [SerializeField] private int EffectId;
        [SerializeField] private bool Loop;
        [SerializeField] private bool ManualAdvanceFrame;
        [HideInInspector] [SerializeField] private int newFrame;

        private bool isInit;
        private float _time;
        private int _currentFrame;
        
        private EffectRenderInfo _effectRenderInfo;
        private AudioSource _audioSource;

        public UnityAction OnEnd;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Initialize(EffectRenderInfo renderInfo)
        {
            isInit = false;
            _time = 0;
            _currentFrame = 0;
            _effectRenderInfo = renderInfo;
            isInit = true;

            if (renderInfo.AudioClip != null)
            {
                _audioSource.clip = _effectRenderInfo.AudioClip;
                _audioSource.Play();
            }
        }

        public override void ManagedUpdate()
        {
            if (!isInit)
                return;

            _time += Time.deltaTime;
            if (!ManualAdvanceFrame)
                newFrame = Mathf.FloorToInt(_time * _effectRenderInfo.Fps);

            Render();

            if (newFrame == _currentFrame) return;
            _currentFrame = newFrame;
            if (_currentFrame <= _effectRenderInfo.MaxKey) return;

            isInit = false;
            _audioSource.clip = null;
            if (Loop)
            {
                _time = 0;
                _currentFrame = -1;
                isInit = true;
            }

            OnEnd?.Invoke();
        }

        private void Render()
        {
            if (!_effectRenderInfo.Frames.TryGetValue(_currentFrame, out var value)) return;
            foreach (var pInfo in value)
            {
                Graphics.RenderMesh(pInfo.RenderParams, pInfo.Mesh, 0, transform.localToWorldMatrix);
            }
        }

        public void Replay()
        {
            if (_effectRenderInfo != null)
                Initialize(_effectRenderInfo);
        }
    }
}