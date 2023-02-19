using UnityEngine;

public abstract class Effect : ScriptableObject {
    public int EffectId;
    
    public long duration;
    public int duplicates;
    public float timeBetweenDuplication;
    public AudioClip wav;
    public bool attachedEntity;
}