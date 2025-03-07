﻿public enum EntityStatus : int {
    SP_SPEED, SP_BASEEXP, SP_JOBEXP, SP_KARMA, SP_MANNER, SP_HP, SP_MAXHP, SP_SP,   // 0-7
    SP_MAXSP, SP_STATUSPOINT, SP_0a, SP_BASELEVEL, SP_SKILLPOINT, SP_STR, SP_AGI, SP_VIT,   // 8-15
    SP_INT, SP_DEX, SP_LUK, SP_CLASS, SP_ZENY, SP_SEX, SP_NEXTBASEEXP, SP_NEXTJOBEXP,   // 16-23
    SP_WEIGHT, SP_MAXWEIGHT, SP_1a, SP_1b, SP_1c, SP_1d, SP_1e, SP_1f,  // 24-31
    SP_USTR, SP_UAGI, SP_UVIT, SP_UINT, SP_UDEX, SP_ULUK, SP_26, SP_27, // 32-39
    SP_28, SP_ATK1, SP_ATK2, SP_MATK1, SP_MATK2, SP_DEF1, SP_DEF2, SP_MDEF1,    // 40-47
    SP_MDEF2, SP_HIT, SP_FLEE1, SP_FLEE2, SP_CRITICAL, SP_ASPD, SP_36, SP_JOBLEVEL, // 48-55
    SP_UPPER, SP_PARTNER, SP_CART, SP_FAME, SP_UNBREAKABLE, //56-60
    SP_CARTINFO = 99,   // 99

    SP_KILLEDGID = 118,
    SP_BASEJOB = 119,   // 100+19 - celest
    SP_BASECLASS = 120, //Hmm.. why 100+19? I just use the next one... [Skotlex]
    SP_KILLERRID = 121,
    SP_KILLEDRID = 122,
    SP_SITTING = 123,
    SP_CHARMOVE = 124,
    SP_CHARRENAME = 125,
    SP_CHARFONT = 126,
    SP_BANK_VAULT = 127,
    SP_ROULETTE_BRONZE = 128,
    SP_ROULETTE_SILVER = 129,
    SP_ROULETTE_GOLD = 130,
    SP_CASHPOINTS, SP_KAFRAPOINTS,
    SP_PCDIECOUNTER, SP_COOKMASTERY,
    SP_ACHIEVEMENT_LEVEL,

    // Mercenaries
    SP_MERCFLEE = 165, SP_MERCKILLS = 189, SP_MERCFAITH = 190,
    
    // 4th jobs
    SP_POW=219, SP_STA, SP_WIS, SP_SPL, SP_CON, SP_CRT,	// 219-224
    SP_PATK, SP_SMATK, SP_RES, SP_MRES, SP_HPLUS, SP_CRATE,	// 225-230
    SP_TRAITPOINT, SP_AP, SP_MAXAP,	// 231-233
    SP_UPOW=247, SP_USTA, SP_UWIS, SP_USPL, SP_UCON, SP_UCRT,	// 247-252

    // original 1000-
    SP_ATTACKRANGE = 1000, SP_ATKELE, SP_DEFELE,    // 1000-1002
    SP_CASTRATE, SP_MAXHPRATE, SP_MAXSPRATE, SP_SPRATE, // 1003-1006
    SP_ADDELE, SP_ADDRACE, SP_ADDSIZE, SP_SUBELE, SP_SUBRACE, // 1007-1011
    SP_ADDEFF, SP_RESEFF,   // 1012-1013
    SP_BASE_ATK, SP_ASPD_RATE, SP_HP_RECOV_RATE, SP_SP_RECOV_RATE, SP_SPEED_RATE, // 1014-1018
    SP_CRITICAL_DEF, SP_NEAR_ATK_DEF, SP_LONG_ATK_DEF, // 1019-1021
    SP_DOUBLE_RATE, SP_DOUBLE_ADD_RATE, SP_SKILL_HEAL, SP_MATK_RATE, // 1022-1025
    SP_IGNORE_DEF_ELE, SP_IGNORE_DEF_RACE, // 1026-1027
    SP_ATK_RATE, SP_SPEED_ADDRATE, SP_SP_REGEN_RATE, // 1028-1030
    SP_MAGIC_ATK_DEF, SP_MISC_ATK_DEF, // 1031-1032
    SP_IGNORE_MDEF_ELE, SP_IGNORE_MDEF_RACE, // 1033-1034
    SP_MAGIC_ADDELE, SP_MAGIC_ADDRACE, SP_MAGIC_ADDSIZE, // 1035-1037
    SP_PERFECT_HIT_RATE, SP_PERFECT_HIT_ADD_RATE, SP_CRITICAL_RATE, SP_GET_ZENY_NUM, SP_ADD_GET_ZENY_NUM, // 1038-1042
    SP_ADD_DAMAGE_CLASS, SP_ADD_MAGIC_DAMAGE_CLASS, SP_ADD_DEF_MONSTER, SP_ADD_MDEF_MONSTER, // 1043-1046
    SP_ADD_MONSTER_DROP_ITEM, SP_DEF_RATIO_ATK_ELE, SP_DEF_RATIO_ATK_RACE, SP_UNBREAKABLE_GARMENT, // 1047-1050
    SP_HIT_RATE, SP_FLEE_RATE, SP_FLEE2_RATE, SP_DEF_RATE, SP_DEF2_RATE, SP_MDEF_RATE, SP_MDEF2_RATE, // 1051-1057
    SP_SPLASH_RANGE, SP_SPLASH_ADD_RANGE, SP_AUTOSPELL, SP_HP_DRAIN_RATE, SP_SP_DRAIN_RATE, // 1058-1062
    SP_SHORT_WEAPON_DAMAGE_RETURN, SP_LONG_WEAPON_DAMAGE_RETURN, SP_WEAPON_COMA_ELE, SP_WEAPON_COMA_RACE, // 1063-1066
    SP_ADDEFF2, SP_BREAK_WEAPON_RATE, SP_BREAK_ARMOR_RATE, SP_ADD_STEAL_RATE, // 1067-1070
    SP_MAGIC_DAMAGE_RETURN, SP_ALL_STATS = 1073, SP_AGI_VIT, SP_AGI_DEX_STR, SP_PERFECT_HIDE, // 1071-1076
    SP_NO_KNOCKBACK, SP_CLASSCHANGE, // 1077-1078
    SP_HP_DRAIN_VALUE, SP_SP_DRAIN_VALUE, // 1079-1080
    SP_WEAPON_ATK, SP_WEAPON_DAMAGE_RATE, // 1081-1082
    SP_DELAYRATE, SP_HP_DRAIN_VALUE_RACE, SP_SP_DRAIN_VALUE_RACE, // 1083-1085
    SP_IGNORE_MDEF_RACE_RATE, SP_IGNORE_DEF_RACE_RATE, SP_SKILL_HEAL2, SP_ADDEFF_ONSKILL, //1086-1089
    SP_ADD_HEAL_RATE, SP_ADD_HEAL2_RATE, SP_EQUIP_ATK, //1090-1092

    SP_RESTART_FULL_RECOVER = 2000, SP_NO_CASTCANCEL, SP_NO_SIZEFIX, SP_NO_MAGIC_DAMAGE, SP_NO_WEAPON_DAMAGE, SP_NO_GEMSTONE, // 2000-2005
    SP_NO_CASTCANCEL2, SP_NO_MISC_DAMAGE, SP_UNBREAKABLE_WEAPON, SP_UNBREAKABLE_ARMOR, SP_UNBREAKABLE_HELM, // 2006-2010
    SP_UNBREAKABLE_SHIELD, SP_LONG_ATK_RATE, // 2011-2012

    SP_CRIT_ATK_RATE, SP_CRITICAL_ADDRACE, SP_NO_REGEN, SP_ADDEFF_WHENHIT, SP_AUTOSPELL_WHENHIT, // 2013-2017
    SP_SKILL_ATK, SP_UNSTRIPABLE, SP_AUTOSPELL_ONSKILL, // 2018-2020
    SP_SP_GAIN_VALUE, SP_HP_REGEN_RATE, SP_HP_LOSS_RATE, SP_ADDRACE2, SP_HP_GAIN_VALUE, // 2021-2025
    SP_SUBSIZE, SP_HP_DRAIN_VALUE_CLASS, SP_ADD_ITEM_HEAL_RATE, SP_SP_DRAIN_VALUE_CLASS, SP_EXP_ADDRACE,    // 2026-2030
    SP_SP_GAIN_RACE, SP_SUBRACE2, SP_UNBREAKABLE_SHOES, // 2031-2033
    SP_UNSTRIPABLE_WEAPON, SP_UNSTRIPABLE_ARMOR, SP_UNSTRIPABLE_HELM, SP_UNSTRIPABLE_SHIELD,  // 2034-2037
    SP_INTRAVISION, SP_ADD_MONSTER_DROP_ITEMGROUP, SP_SP_LOSS_RATE, // 2038-2040
    SP_ADD_SKILL_BLOW, SP_SP_VANISH_RATE, SP_MAGIC_SP_GAIN_VALUE, SP_MAGIC_HP_GAIN_VALUE, SP_ADD_MONSTER_ID_DROP_ITEM, //2041-2045
    SP_EMATK, SP_COMA_CLASS, SP_COMA_RACE, SP_SKILL_USE_SP_RATE, //2046-2049
    SP_SKILL_COOLDOWN, SP_SKILL_FIXEDCAST, SP_SKILL_VARIABLECAST, SP_FIXCASTRATE, SP_VARCASTRATE, //2050-2054
    SP_SKILL_USE_SP, SP_MAGIC_ATK_ELE, SP_ADD_FIXEDCAST, SP_ADD_VARIABLECAST,  //2055-2058
    SP_SET_DEF_RACE, SP_SET_MDEF_RACE, SP_HP_VANISH_RATE,  //2059-2061

    SP_IGNORE_DEF_CLASS, SP_DEF_RATIO_ATK_CLASS, SP_ADDCLASS, SP_SUBCLASS, SP_MAGIC_ADDCLASS, //2062-2066
    SP_WEAPON_COMA_CLASS, SP_IGNORE_MDEF_CLASS_RATE, SP_EXP_ADDCLASS, SP_ADD_CLASS_DROP_ITEM, //2067-2070
    SP_ADD_CLASS_DROP_ITEMGROUP, SP_ADDMAXWEIGHT, SP_ADD_ITEMGROUP_HEAL_RATE,  // 2071-2073
    SP_HP_VANISH_RACE_RATE, SP_SP_VANISH_RACE_RATE, SP_ABSORB_DMG_MAXHP, SP_SUB_SKILL, SP_SUBDEF_ELE, // 2074-2078
    SP_STATE_NORECOVER_RACE, SP_CRITICAL_RANGEATK, SP_MAGIC_ADDRACE2, SP_IGNORE_MDEF_RACE2_RATE, // 2079-2082
    SP_WEAPON_ATK_RATE, SP_WEAPON_MATK_RATE, SP_DROP_ADDRACE, SP_DROP_ADDCLASS, SP_NO_MADO_FUEL, // 2083-2087
    SP_IGNORE_DEF_CLASS_RATE, SP_REGEN_PERCENT_HP, SP_REGEN_PERCENT_SP, SP_SKILL_DELAY, SP_NO_WALK_DELAY, //2088-2093
    SP_LONG_SP_GAIN_VALUE, SP_LONG_HP_GAIN_VALUE // 2094-2095
}