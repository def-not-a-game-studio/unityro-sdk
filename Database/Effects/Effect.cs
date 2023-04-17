using System;
using Assets.Scripts.Renderer.Map.Effects.EffectParts;
using UnityEngine;

[CreateAssetMenu(menuName = "UnityRO/Database Entry/Effect")]
[Serializable]
public class Effect : ScriptableObject {
    
    public int EffectId;
    
    public CylinderEffectPart[] CylinderParts;
    public SprEffect[] SPRParts;
    public StrEffect[] STRParts;
    public ThreeDEffect[] ThreeDParts;
    public TwoDEffect[] TwoDParts;
}