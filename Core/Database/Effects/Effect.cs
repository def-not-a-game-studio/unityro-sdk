using System;
using UnityEngine;

namespace Core.Effects.EffectParts {
    [CreateAssetMenu(menuName = "Database Entry/Effect")]
    [Serializable]
    public class Effect : ScriptableObject {
        public int EffectId;

        public CylinderEffectPart[] CylinderParts;
        public SprEffectPart[] SPRParts;
        public StrEffectPart[] STRParts;
        public ThreeDEffectPart[] ThreeDParts;
        public TwoDEffectPart[] TwoDParts;
    }
}