using System;
using System.Collections.Generic;
using Core.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace UnityRO.Core.Audio
{
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        IObjectPool<SoundEmitter> soundEmitterPool;
        readonly List<SoundEmitter> activeSoundEmitters = new();
        
        public readonly Dictionary<string, LinkedList<SoundEmitter>> FrequentSoundEmittersMap = new();
        
        [SerializeField] SoundEmitter soundEmitterPrefab;
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        [SerializeField] int maxSoundInstances = 15;

        private void Start()
        {
            InitializePool();
        }
        
        public SoundBuilder CreateSoundBuilder() => new SoundBuilder(this);

        public bool CanPlaySound(SoundData data) {
            if (!data.frequentSound) return true;

            if (FrequentSoundEmittersMap.TryGetValue(data.ClipName, out var frequentSoundEmitters))
            {
                if (frequentSoundEmitters.Count >= maxSoundInstances)
                {
                    try
                    {
                        frequentSoundEmitters.First.Value.Stop();
                        return true;
                    }
                    catch
                    {
                        Debug.Log("SoundEmitter is already released");
                    }

                    return false;
                }
            }
            return true;
        }

        public SoundEmitter Get() {
            return soundEmitterPool.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter) {
            soundEmitterPool.Release(soundEmitter);
        }

        public void StopAll() {
            foreach (var soundEmitter in activeSoundEmitters) {
                soundEmitter.Stop();
            }

            FrequentSoundEmittersMap.Clear();
        }

        void InitializePool()
        {
            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize);
        }
        
        SoundEmitter CreateSoundEmitter() {
            var soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        void OnTakeFromPool(SoundEmitter soundEmitter) {
            soundEmitter.gameObject.SetActive(true);
            activeSoundEmitters.Add(soundEmitter);
        }

        void OnReturnedToPool(SoundEmitter soundEmitter) {
            if (soundEmitter.Node != null) {
                if (FrequentSoundEmittersMap.TryGetValue(soundEmitter.Data.ClipName, out var soundEmitters))
                {
                    soundEmitters.Remove(soundEmitter.Node);
                    if (soundEmitters.Count == 0)
                    {
                        FrequentSoundEmittersMap.Remove(soundEmitter.Data.ClipName);
                    }
                }
                soundEmitter.Node = null;
            }
            soundEmitter.gameObject.SetActive(false);
            activeSoundEmitters.Remove(soundEmitter);
        }

        void OnDestroyPoolObject(SoundEmitter soundEmitter) {
            Destroy(soundEmitter.gameObject);
        }

        public void AddFrequentSoundEmitter(SoundEmitter soundEmitter)
        {
            if (FrequentSoundEmittersMap.TryGetValue(soundEmitter.Data.ClipName, out var frequentSoundEmitters))
            {
                soundEmitter.Node = frequentSoundEmitters.AddLast(soundEmitter);
            }
            else
            {
                var list = new LinkedList<SoundEmitter>();
                FrequentSoundEmittersMap.Add(soundEmitter.Data.ClipName, list);
                soundEmitter.Node = list.AddLast(soundEmitter);
            }
        }
    }
}