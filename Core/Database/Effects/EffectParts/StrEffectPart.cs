using System;
using ROIO.Models.FileTypes;
using UnityEngine;

namespace Core.Effects.EffectParts {
    [Serializable]
    public class StrEffectPart : EffectPart {
        public STR file;
        public STR[] files; // if rand is present we load all
        public STR simplified; // min
        public AudioClip wav;
        public AudioClip[] wavs;
        public bool attachedEntity;
        public bool head;
        public bool repeat;
    }
}