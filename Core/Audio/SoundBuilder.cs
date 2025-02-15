using UnityEngine;

namespace UnityRO.Core.Audio
{
    public class SoundBuilder
    {
        private readonly SoundManager soundManager;
        
        Vector3 position = Vector3.zero;
        private bool randomPitch;

        public SoundBuilder(SoundManager manager)
        {
            soundManager = manager;
        }

        public SoundBuilder WithPosition(Vector3 position)
        {
            this.position = position;
            return this;
        }
        
        public SoundBuilder WithRandomPitch()
        {
            randomPitch = true;
            return this;
        }

        public void Play(SoundData soundData)
        {
            if (soundData == null)
            {
                Debug.LogError("SoundData is null");
                return;
            }

            if (!soundManager.CanPlaySound(soundData)) return;

            SoundEmitter soundEmitter = soundManager.Get();
            soundEmitter.Initialize(soundData);
            soundEmitter.transform.position = position;
            soundEmitter.transform.parent = soundManager.transform;

            if (randomPitch)
            {
                soundEmitter.WithRandomPitch();
            }

            if (soundData.frequentSound)
            {
                soundManager.AddFrequentSoundEmitter(soundEmitter);
            }

            soundEmitter.Play();
        }
    }
}