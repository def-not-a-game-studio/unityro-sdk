using Core.Effects;
using UnityEngine;

[CreateAssetMenu(menuName = "MidgardSurvivors/Database Entry/Skill")]
public class Skill : ScriptableObject {
    public int SkillId;
    public SkillEffect Effect;

    public int BaseDamage;
}