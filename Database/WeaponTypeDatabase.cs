namespace UnityRO.Core.Database {
    public enum WeaponType : int {
        None = 0,
        ShortSword = 1,
        Sword = 2,
        TwoHandSword = 3,
        Spear = 4,
        TwoHandSpear = 5,
        Axe = 6,
        TwoHandAxe = 7,
        Mace = 8,
        TwoHandMace = 9,
        Rod = 10,
        Bow = 11,
        Knuckle = 12,
        Instrument = 13,
        Whip = 14,
        Book = 15,
        Catarrh = 16,
        GunHandgun = 17,
        GunRifle = 18,
        GunGatling = 19,
        GunShotgun = 20,
        GunGrenade = 21,
        Shuriken = 22,
        TwoHandRod = 23,
        LastClass = 24,
        ShortSwordShortSword = 25,
        SwordSword = 26,
        AxeAxe = 27,
        ShortSwordSword = 28,
        ShortSwordAxe = 29,
        SwordAxe = 30,
        Last = 31,
    }

    public static class WeaponTypeDatabase {
        public static WeaponType GetWeaponType(int itemID) {
            if (itemID <= 0) return WeaponType.None;

            var type = itemID switch {
                >= 1100 and < 1150 => 2,
                >= 13400 and < 13500 => 2,
                >= 1150 and < 1200 => 3,
                >= 1150 and < 1250 => 1,
                >= 1150 and < 1300 => 16,
                >= 1150 and < 1350 => 6,
                >= 1150 and < 1400 => 7,
                >= 1150 and < 1450 => 4,
                >= 1150 and < 1500 when itemID != 1472 && itemID != 1473 => 5,
                >= 1150 and < 1500 => 10,
                >= 1150 and < 1550 => 8,
                >= 1150 and < 1600 => 15,
                >= 1150 and < 1700 => 10,
                >= 1150 and < 1750 => 11,
                >= 1800 and < 1900 => 12,
                >= 1800 and < 1950 => 13,
                >= 1800 and < 2000 => 14,
                >= 1800 and < 2100 => 23,
                >= 13150 and < 13200 => 18,
                < 13000 => itemID,
                < 13100 => 1,
                >= 13150 and < 13300 or >= 13400 => -1,
                >= 13150 => 22,
                _ => 17
            };

            return (WeaponType)type;
        }
    }
}