using System;
using Assets.Scripts.Renderer.Map.Effects.EffectParts;
using UnityEngine;

namespace Assets.Scripts.Renderer.Map.Effects.EffectParts {
    
    [Serializable]
    public class CylinderEffectPart : EffectPart {
        // name of texture from data/texture/effect/*.tga
        public Texture2D texture;
        public Color Color = Color.white;
        public int totalCircleSides = 20;
        public int circleSides = 20;
        public int repeatTextureX;
        public bool rotate;
        public float angleX;
        public float angleY;
        public float angleZ;
        public bool rotateWithCamera;
        public bool rotateToTarget;
        public bool rotateWithSource;
        public float topSize;
        public float bottomSize;
        public float height;
        public Vector3 position;
        public bool fade;

        ///	 - 1: the height of the cylinder grows to the set height from 0
        ///	 - 2: top radius of the cylinder grows to the topSize value from 0
        ///	 - 3: the radius of the whole cylinder shrinks to 0
        ///	 - 4: the radius of the whole cylinder grows to the set topSize and bottomSize values from 0
        ///	 - 5: the height of the cylinder grows to the set height value, and then shrinks back to 0
        public int animation;

        ///	 - 1: ZERO
        ///	 - 2: ONE (transparent light effect)
        ///	 - 3: SRC_COLOR
        ///	 - 4: ONE_MINUS_SRC_COLOR
        ///	 - 5: DST_COLOR
        ///	 - 6: ONE_MINUS_DST_COLOR
        ///	 - 7: SRC_ALPHA
        ///	 - 8: ONE_MINUS_SRC_ALPHA (default)
        ///	 - 9: DST_ALPHA
        ///	 - 10: ONE_MINUS_DST_ALPHA
        ///	 - 11: CONSTANT_COLOR
        ///	 - 12: ONE_MINUS_CONSTANT_COLOR
        ///	 - 13: CONSTANT_ALPHA
        ///	 - 14: ONE_MINUS_CONSTANT_ALPHA
        ///	 - 15: SRC_ALPHA_SATURATE
        public int blendMode;

        public bool repeat;
        public long duration;
        public int duplicates;
        public long timeBetweenDuplication;
        public AudioClip wav;
        public bool attachedEntity;
        public float alphaMax = 1f;
    }
}