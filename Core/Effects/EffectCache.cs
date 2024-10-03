using System;
using System.Collections.Generic;
using UnityEngine;

namespace _3rdparty.unityro_sdk.Core.Effects
{
    public class EffectRenderInfo
    {
        public RenderParams RenderParams;
        public Mesh Mesh;
    }
    
    public class EffectCache : MonoBehaviour
    {
        public Dictionary<int, Dictionary<int, List<EffectRenderInfo>>> EffectRenderInfos = new();

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void CacheEffect(int effectId, Dictionary<int, List<EffectRenderInfo>> effectInfo)
        {
            EffectRenderInfos.TryAdd(effectId, effectInfo);
        }

        public Dictionary<int, List<EffectRenderInfo>> GetRenderInfo(int effectId)
        {
            EffectRenderInfos.TryGetValue(effectId, out var effectRenderInfo);
            return effectRenderInfo;
        }
    }
}