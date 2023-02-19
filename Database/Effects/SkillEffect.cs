using UnityEngine;

namespace Assets.Scripts.Renderer.Map.Effects
{
    [CreateAssetMenu(menuName = "MidgardSurvivors/Database Entry/Skill Effect")]
    public class SkillEffect : ScriptableObject
    {
        public Effect Effect;
        public Effect CasterEffect;
        public Effect GroundEffect;
        public Effect HitEffect;
        public Effect BeforeHitEffect;
        public Effect BeginCastEffect;
        public Effect SuccessEffect;
        public Effect CasterSuccessEffect;

        public bool hideCastBar;
        public bool hideCastAura;
    }
}