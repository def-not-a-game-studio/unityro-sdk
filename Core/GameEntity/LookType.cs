namespace UnityRO.Core.GameEntity {
    public enum LookType : int {
        LOOK_BASE,
        LOOK_HAIR,
        LOOK_WEAPON,
        LOOK_HEAD_BOTTOM,
        LOOK_HEAD_TOP,
        LOOK_HEAD_MID,
        LOOK_HAIR_COLOR,
        LOOK_CLOTHES_COLOR,
        LOOK_SHIELD,
        LOOK_SHOES,
        LOOK_BODY,          //Purpose Unknown. Doesen't appear to do anything.
        LOOK_RESET_COSTUMES,//Makes all headgear sprites on player vanish when activated.
        LOOK_ROBE,
        // LOOK_FLOOR,	// TODO : fix me!! offcial use this ?
        LOOK_BODY2
    };
}