using System.Collections.Generic;
using UnityEngine;

public class GameEntitySkillController : MonoBehaviour {

    private SkillsDatabase SkillsDatabase;

    private Skill CurrentSkill;

    private Dictionary<Skill, GameObject> CurrentSpawnedSkills;

    // Start is called before the first frame update
    void Start() {
        SkillsDatabase = Resources.Load<SkillsDatabase>("Database/SkillsDatabase");
        CurrentSkill = SkillsDatabase.Values[0];
        CurrentSpawnedSkills = new();
    }

    // Update is called once per frame
    void Update() {
        if (Random.Range(0,2) == 1 && !CurrentSpawnedSkills.ContainsKey(CurrentSkill))
        {
            var effect = CurrentSkill.Effect;
            var skill = Instantiate((effect.CasterEffect as ThreeDEffect).EffectObject, transform, true);
            CurrentSpawnedSkills[CurrentSkill] = skill;
        }
    }
}
