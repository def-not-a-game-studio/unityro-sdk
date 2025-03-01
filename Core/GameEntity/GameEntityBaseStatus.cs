using System;
using UnityEngine.Serialization;

[Serializable]
public class GameEntityBaseStatus {
    
    #region Style
    public int HairColor;
    public int ClothesColor;
    public int HairStyle;
    public int Job;
    public bool IsMale;
    #endregion

    public EntityType EntityType;
        
    public int GID;
    public int AID;
    public int GUID;
    public string Name;

    public int MoveSpeed;
    public int AttackSpeed;
    public int AttackedSpeed;

    public int Weapon;
    public int Shield;

    public long HP;
    public long MaxHP;
    public long SP;
    public long MaxSP;
    public long Money;
    public int BaseLevel;
    public int JobLevel;

    public short SkillPoints;
    public short StatusPoints;
    public int Str;
    public int Agi;
    public int Vit;
    public int Int;
    public int Dex;
    public int Luk;
    public int NeedStr;
    public int NeedAgi;
    public int NeedVit;
    public int NeedInt;
    public int NeedDex;
    public int NeedLuk;
    public short Atk;
    public short Atk2;
    public short MatkMin;
    public short MatkMax;
    public short Def;
    public short Def2;
    public short Mdef;
    public short Mdef2;
    public short Hit;
    public short Flee;
    public short Flee2;
    public short Crit;
    public short Aspd;
    public short Aspd2;
    
    // 4th jobs
    public short Patk;
    public short Smatk;
    public short Res;
    public short Mres;
    public short Hplus;
    public short Crate;
    public short TraitPoints;
    public short Ap;
    public short MaxAp;

    public long BaseExp;
    public long NextBaseExp;
    public long JobExp;
    public long NextJobExp;

    public long Weight;
    public long MaxWeight;
}