using System;
using _3rdparty.unityro_sdk.Core.Effects;
using UnityEngine;
using UnityEngine.Events;
using UnityRO.Core;

namespace Core.Effects
{
    public class StrEffectRenderer : EffectRendererPart
    {
        private int _newFrame;
        private int _currentFrame;
        private bool _isInit;
        private float _time;

        private EffectRenderInfo _effectRenderInfo;

        public void Initialize(EffectRenderInfo renderInfo)
        {
            _isInit = false;
            _time = 0;
            _currentFrame = 0;
            _effectRenderInfo = renderInfo;
            _isInit = true;

            if (renderInfo.AudioClip is not null)
            {
                OnAudio?.Invoke(renderInfo.AudioClip);
            }
        }

        public override void Update(Matrix4x4 matrix)
        {
            if (!_isInit)
                return;

            _time += Time.deltaTime;
            _newFrame = Mathf.FloorToInt(_time * _effectRenderInfo.Fps);

            Render(matrix);

            if (_newFrame == _currentFrame) return;
            _currentFrame = _newFrame;
            if (_currentFrame <= _effectRenderInfo.MaxKey) return;

            _isInit = false;

            OnEnd?.Invoke(this);
        }

        private void Render(Matrix4x4 matrix)
        {
            if (!_effectRenderInfo.Frames.TryGetValue(_currentFrame, out var value)) return;
            foreach (var pInfo in value)
            {
                Graphics.RenderMesh(pInfo.RenderParams, pInfo.Mesh, 0, matrix);
            }
        }
    }
}