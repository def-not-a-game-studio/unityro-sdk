using System;

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
    public int Level;
}