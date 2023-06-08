using System.Collections.Generic;
using UnityEditor;
using UnityRO.Core.Database;

namespace UnityRO.Core.Editor {
    [InitializeOnLoad]
    public static class SpriteUtilityTables {
        public const int JOBMINUS = 3950;

        static SpriteUtilityTables() {
            InitPcJobNameTable();
        }

        public static Dictionary<int, string> m_newPcJobNameTable = new();

        private static void InitPcJobNameTable() {
            m_newPcJobNameTable[(int)JobType.JT_NOVICE] = "ÃÊº¸ÀÚ";

            m_newPcJobNameTable[(int)JobType.JT_SANTA] = "»êÅ¸";
            m_newPcJobNameTable[(int)JobType.JT_SUMMER] = "¿©¸§";
            m_newPcJobNameTable[(int)JobType.JT_HANBOK] = "ÇÑº¹";
            m_newPcJobNameTable[(int)JobType.JT_OKTOBERFEST] = "¿ÁÅä¹öÆÐ½ºÆ®";
            m_newPcJobNameTable[(int)JobType.JT_SUMMER2] = "¿©¸§2";

            m_newPcJobNameTable[(int)JobType.JT_SWORDMAN] = "°Ë»ç";
            m_newPcJobNameTable[(int)JobType.JT_MAGICIAN] = "¸¶¹ý»ç";
            m_newPcJobNameTable[(int)JobType.JT_ARCHER] = "±Ã¼ö";
            m_newPcJobNameTable[(int)JobType.JT_ACOLYTE] = "¼ºÁ÷ÀÚ";
            m_newPcJobNameTable[(int)JobType.JT_MERCHANT] = "»óÀÎ";
            m_newPcJobNameTable[(int)JobType.JT_THIEF] = "µµµÏ";
            m_newPcJobNameTable[(int)JobType.JT_KNIGHT] = "±â»ç";
            m_newPcJobNameTable[(int)JobType.JT_PRIEST] = "ÇÁ¸®½ºÆ®";
            m_newPcJobNameTable[(int)JobType.JT_WIZARD] = "À§Àúµå";
            m_newPcJobNameTable[(int)JobType.JT_BLACKSMITH] = "Á¦Ã¶°ø";
            m_newPcJobNameTable[(int)JobType.JT_HUNTER] = "ÇåÅÍ";
            m_newPcJobNameTable[(int)JobType.JT_ASSASSIN] = "¾î¼¼½Å";
            m_newPcJobNameTable[(int)JobType.JT_CHICKEN] = "ÆäÄÚÆäÄÚ_±â»ç";
            m_newPcJobNameTable[(int)JobType.JT_CRUSADER] = "Å©·ç¼¼ÀÌ´õ";
            m_newPcJobNameTable[(int)JobType.JT_MONK] = "¸ùÅ©";
            m_newPcJobNameTable[(int)JobType.JT_SAGE] = "¼¼ÀÌÁö";
            m_newPcJobNameTable[(int)JobType.JT_ROGUE] = "·Î±×";
            m_newPcJobNameTable[(int)JobType.JT_ALCHEMIST] = "¿¬±Ý¼ú»ç";
            m_newPcJobNameTable[(int)JobType.JT_BARD] = "¹Ùµå";
            m_newPcJobNameTable[(int)JobType.JT_DANCER] = "¹«Èñ";
            m_newPcJobNameTable[(int)JobType.JT_CHICKEN2] = "½ÅÆäÄÚÅ©·ç¼¼ÀÌ´õ";
            m_newPcJobNameTable[(int)JobType.JT_SUPERNOVICE] = "½´ÆÛ³ëºñ½º";
            m_newPcJobNameTable[(int)JobType.JT_MARRIED] = "°áÈ¥";

            m_newPcJobNameTable[(int)JobType.JT_GUNSLINGER] = "°Ç³Ê";
            m_newPcJobNameTable[(int)JobType.JT_NINJA] = "´ÑÀÚ";

            m_newPcJobNameTable[(int)JobType.JT_NOVICE_H - JOBMINUS] = "ÃÊº¸ÀÚ";
            m_newPcJobNameTable[(int)JobType.JT_SWORDMAN_H - JOBMINUS] = "°Ë»ç";
            m_newPcJobNameTable[(int)JobType.JT_MAGICIAN_H - JOBMINUS] = "¸¶¹ý»ç";
            m_newPcJobNameTable[(int)JobType.JT_ARCHER_H - JOBMINUS] = "±Ã¼ö";
            m_newPcJobNameTable[(int)JobType.JT_ACOLYTE_H - JOBMINUS] = "¼ºÁ÷ÀÚ";
            m_newPcJobNameTable[(int)JobType.JT_MERCHANT_H - JOBMINUS] = "»óÀÎ";
            m_newPcJobNameTable[(int)JobType.JT_THIEF_H - JOBMINUS] = "µµµÏ";

            m_newPcJobNameTable[(int)JobType.JT_KNIGHT_H - JOBMINUS] = "·Îµå³ªÀÌÆ®";
            m_newPcJobNameTable[(int)JobType.JT_PRIEST_H - JOBMINUS] = "ÇÏÀÌÇÁ¸®";
            m_newPcJobNameTable[(int)JobType.JT_WIZARD_H - JOBMINUS] = "ÇÏÀÌÀ§Àúµå";
            m_newPcJobNameTable[(int)JobType.JT_BLACKSMITH_H - JOBMINUS] = "È­ÀÌÆ®½º¹Ì½º";
            m_newPcJobNameTable[(int)JobType.JT_HUNTER_H - JOBMINUS] = "½º³ªÀÌÆÛ";
            m_newPcJobNameTable[(int)JobType.JT_ASSASSIN_H - JOBMINUS] = "¾î½Ø½ÅÅ©·Î½º";
            m_newPcJobNameTable[(int)JobType.JT_CHICKEN_H - JOBMINUS] = "·ÎµåÆäÄÚ";

            m_newPcJobNameTable[(int)JobType.JT_CRUSADER_H - JOBMINUS] = "ÆÈ¶óµò";
            m_newPcJobNameTable[(int)JobType.JT_MONK_H - JOBMINUS] = "Ã¨ÇÇ¿Â";
            m_newPcJobNameTable[(int)JobType.JT_SAGE_H - JOBMINUS] = "ÇÁ·ÎÆä¼­";
            m_newPcJobNameTable[(int)JobType.JT_ROGUE_H - JOBMINUS] = "½ºÅäÄ¿";
            m_newPcJobNameTable[(int)JobType.JT_ALCHEMIST_H - JOBMINUS] = "Å©¸®¿¡ÀÌÅÍ";
            m_newPcJobNameTable[(int)JobType.JT_BARD_H - JOBMINUS] = "Å¬¶ó¿î";
            m_newPcJobNameTable[(int)JobType.JT_DANCER_H - JOBMINUS] = "Áý½Ã";
            m_newPcJobNameTable[(int)JobType.JT_CHICKEN2_H - JOBMINUS] = "ÆäÄÚÆÈ¶óµò";

            m_newPcJobNameTable[(int)JobType.JT_NOVICE_B - JOBMINUS] = "ÃÊº¸ÀÚ";

            m_newPcJobNameTable[(int)JobType.JT_GANGSI - JOBMINUS] = "¼ºÁ÷ÀÚ"; // ¿ì¼± ±âÁ¸²¨
            m_newPcJobNameTable[(int)JobType.JT_DEATHKNIGHT - JOBMINUS] = "±â»ç";
            m_newPcJobNameTable[(int)JobType.JT_COLLECTOR - JOBMINUS] = "¼¼ÀÌÁö";

            m_newPcJobNameTable[(int)JobType.JT_SWORDMAN_B - JOBMINUS] = "°Ë»ç";
            m_newPcJobNameTable[(int)JobType.JT_MAGICIAN_B - JOBMINUS] = "¸¶¹ý»ç";
            m_newPcJobNameTable[(int)JobType.JT_ARCHER_B - JOBMINUS] = "±Ã¼ö";
            m_newPcJobNameTable[(int)JobType.JT_ACOLYTE_B - JOBMINUS] = "¼ºÁ÷ÀÚ";
            m_newPcJobNameTable[(int)JobType.JT_MERCHANT_B - JOBMINUS] = "»óÀÎ";
            m_newPcJobNameTable[(int)JobType.JT_THIEF_B - JOBMINUS] = "µµµÏ";
            m_newPcJobNameTable[(int)JobType.JT_KNIGHT_B - JOBMINUS] = "±â»ç";
            m_newPcJobNameTable[(int)JobType.JT_PRIEST_B - JOBMINUS] = "ÇÁ¸®½ºÆ®";
            m_newPcJobNameTable[(int)JobType.JT_WIZARD_B - JOBMINUS] = "À§Àúµå";
            m_newPcJobNameTable[(int)JobType.JT_BLACKSMITH_B - JOBMINUS] = "Á¦Ã¶°ø";
            m_newPcJobNameTable[(int)JobType.JT_HUNTER_B - JOBMINUS] = "ÇåÅÍ";
            m_newPcJobNameTable[(int)JobType.JT_ASSASSIN_B - JOBMINUS] = "¾î¼¼½Å";
            m_newPcJobNameTable[(int)JobType.JT_CHICKEN_B - JOBMINUS] = "ÆäÄÚÆäÄÚ_±â»ç";
            m_newPcJobNameTable[(int)JobType.JT_CRUSADER_B - JOBMINUS] = "Å©·ç¼¼ÀÌ´õ";
            m_newPcJobNameTable[(int)JobType.JT_MONK_B - JOBMINUS] = "¸ùÅ©";
            m_newPcJobNameTable[(int)JobType.JT_SAGE_B - JOBMINUS] = "¼¼ÀÌÁö";
            m_newPcJobNameTable[(int)JobType.JT_ROGUE_B - JOBMINUS] = "·Î±×";
            m_newPcJobNameTable[(int)JobType.JT_ALCHEMIST_B - JOBMINUS] = "¿¬±Ý¼ú»ç";
            m_newPcJobNameTable[(int)JobType.JT_BARD_B - JOBMINUS] = "¹Ùµå";
            m_newPcJobNameTable[(int)JobType.JT_DANCER_B - JOBMINUS] = "¹«Èñ¹ÙÁö";
            m_newPcJobNameTable[(int)JobType.JT_CHICKEN2_B - JOBMINUS] = "±¸ÆäÄÚÅ©·ç¼¼ÀÌ´õ";
            m_newPcJobNameTable[(int)JobType.JT_SUPERNOVICE_B - JOBMINUS] = "½´ÆÛ³ëºñ½º";

            m_newPcJobNameTable[(int)JobType.JT_TAEKWON - JOBMINUS] = "ÅÂ±Ç¼Ò³â";
            m_newPcJobNameTable[(int)JobType.JT_STAR - JOBMINUS] = "±Ç¼º";
            m_newPcJobNameTable[(int)JobType.JT_STAR2 - JOBMINUS] = "±Ç¼ºÀ¶ÇÕ";
            m_newPcJobNameTable[(int)JobType.JT_LINKER - JOBMINUS] = "¼Ò¿ï¸µÄ¿";

            m_newPcJobNameTable[(int)JobType.JT_RUNE_KNIGHT - JOBMINUS] = "·é³ªÀÌÆ®";
            m_newPcJobNameTable[(int)JobType.JT_WARLOCK - JOBMINUS] = "¿ö·Ï";
            m_newPcJobNameTable[(int)JobType.JT_RANGER - JOBMINUS] = "·¹ÀÎÁ®";
            m_newPcJobNameTable[(int)JobType.JT_ARCHBISHOP - JOBMINUS] = "¾ÆÅ©ºñ¼ó";
            m_newPcJobNameTable[(int)JobType.JT_MECHANIC - JOBMINUS] = "¹ÌÄÉ´Ð";
            m_newPcJobNameTable[(int)JobType.JT_GUILLOTINE_CROSS - JOBMINUS] = "±æ·ÎÆ¾Å©·Î½º";

            m_newPcJobNameTable[(int)JobType.JT_RUNE_KNIGHT_H - JOBMINUS] = "·é³ªÀÌÆ®";
            m_newPcJobNameTable[(int)JobType.JT_WARLOCK_H - JOBMINUS] = "¿ö·Ï";
            m_newPcJobNameTable[(int)JobType.JT_RANGER_H - JOBMINUS] = "·¹ÀÎÁ®";
            m_newPcJobNameTable[(int)JobType.JT_ARCHBISHOP_H - JOBMINUS] = "¾ÆÅ©ºñ¼ó";
            m_newPcJobNameTable[(int)JobType.JT_MECHANIC_H - JOBMINUS] = "¹ÌÄÉ´Ð";
            m_newPcJobNameTable[(int)JobType.JT_GUILLOTINE_CROSS_H - JOBMINUS] = "±æ·ÎÆ¾Å©·Î½º";

            m_newPcJobNameTable[(int)JobType.JT_ROYAL_GUARD - JOBMINUS] = "°¡µå";
            m_newPcJobNameTable[(int)JobType.JT_SORCERER - JOBMINUS] = "¼Ò¼­·¯";
            m_newPcJobNameTable[(int)JobType.JT_MINSTREL - JOBMINUS] = "¹Î½ºÆ®·²";
            m_newPcJobNameTable[(int)JobType.JT_WANDERER - JOBMINUS] = "¿ø´õ·¯";
            m_newPcJobNameTable[(int)JobType.JT_SURA - JOBMINUS] = "½´¶ó";
            m_newPcJobNameTable[(int)JobType.JT_GENETIC - JOBMINUS] = "Á¦³×¸¯";
            m_newPcJobNameTable[(int)JobType.JT_SHADOW_CHASER - JOBMINUS] = "½¦µµ¿ìÃ¼ÀÌ¼­";

            m_newPcJobNameTable[(int)JobType.JT_ROYAL_GUARD_H - JOBMINUS] = "°¡µå";
            m_newPcJobNameTable[(int)JobType.JT_SORCERER_H - JOBMINUS] = "¼Ò¼­·¯";
            m_newPcJobNameTable[(int)JobType.JT_MINSTREL_H - JOBMINUS] = "¹Î½ºÆ®·²";
            m_newPcJobNameTable[(int)JobType.JT_WANDERER_H - JOBMINUS] = "¿ø´õ·¯";
            m_newPcJobNameTable[(int)JobType.JT_SURA_H - JOBMINUS] = "½´¶ó";
            m_newPcJobNameTable[(int)JobType.JT_GENETIC_H - JOBMINUS] = "Á¦³×¸¯";
            m_newPcJobNameTable[(int)JobType.JT_SHADOW_CHASER_H - JOBMINUS] = "½¦µµ¿ìÃ¼ÀÌ¼­";

            m_newPcJobNameTable[(int)JobType.JT_RUNE_CHICKEN - JOBMINUS] = "·é³ªÀÌÆ®»Ú¶ì";
            m_newPcJobNameTable[(int)JobType.JT_RUNE_CHICKEN_H - JOBMINUS] = "·é³ªÀÌÆ®»Ú¶ì";
            m_newPcJobNameTable[(int)JobType.JT_ROYAL_CHICKEN - JOBMINUS] = "±×¸®Æù°¡µå";
            m_newPcJobNameTable[(int)JobType.JT_ROYAL_CHICKEN_H - JOBMINUS] = "±×¸®Æù°¡µå";
            m_newPcJobNameTable[(int)JobType.JT_WOLF_RANGER - JOBMINUS] = "·¹ÀÎÁ®´Á´ë";
            m_newPcJobNameTable[(int)JobType.JT_WOLF_RANGER_H - JOBMINUS] = "·¹ÀÎÁ®´Á´ë";
            m_newPcJobNameTable[(int)JobType.JT_MADOGEAR - JOBMINUS] = "¸¶µµ±â¾î";
            m_newPcJobNameTable[(int)JobType.JT_MADOGEAR_H - JOBMINUS] = "¸¶µµ±â¾î";
            m_newPcJobNameTable[(int)JobType.JT_RUNE_CHICKEN2 - JOBMINUS] = "·é³ªÀÌÆ®»Ú¶ì2";
            m_newPcJobNameTable[(int)JobType.JT_RUNE_CHICKEN2_H - JOBMINUS] = "·é³ªÀÌÆ®»Ú¶ì2";
            m_newPcJobNameTable[(int)JobType.JT_RUNE_CHICKEN3 - JOBMINUS] = "·é³ªÀÌÆ®»Ú¶ì3";
            m_newPcJobNameTable[(int)JobType.JT_RUNE_CHICKEN3_H - JOBMINUS] = "·é³ªÀÌÆ®»Ú¶ì3";
            m_newPcJobNameTable[(int)JobType.JT_RUNE_CHICKEN4 - JOBMINUS] = "·é³ªÀÌÆ®»Ú¶ì4";
            m_newPcJobNameTable[(int)JobType.JT_RUNE_CHICKEN4_H - JOBMINUS] = "·é³ªÀÌÆ®»Ú¶ì4";
            m_newPcJobNameTable[(int)JobType.JT_RUNE_CHICKEN5 - JOBMINUS] = "·é³ªÀÌÆ®»Ú¶ì5";
            m_newPcJobNameTable[(int)JobType.JT_RUNE_CHICKEN5_H - JOBMINUS] = "·é³ªÀÌÆ®»Ú¶ì5";

            m_newPcJobNameTable[(int)JobType.JT_RUNE_KNIGHT_B - JOBMINUS] = "·é³ªÀÌÆ®";
            m_newPcJobNameTable[(int)JobType.JT_WARLOCK_B - JOBMINUS] = "¿ö·Ï";
            m_newPcJobNameTable[(int)JobType.JT_RANGER_B - JOBMINUS] = "·¹ÀÎÁ®";
            m_newPcJobNameTable[(int)JobType.JT_ARCHBISHOP_B - JOBMINUS] = "¾ÆÅ©ºñ¼ó";
            m_newPcJobNameTable[(int)JobType.JT_MECHANIC_B - JOBMINUS] = "¹ÌÄÉ´Ð";
            m_newPcJobNameTable[(int)JobType.JT_GUILLOTINE_CROSS_B - JOBMINUS] = "±æ·ÎÆ¾Å©·Î½º";

            m_newPcJobNameTable[(int)JobType.JT_ROYAL_GUARD_B - JOBMINUS] = "°¡µå";
            m_newPcJobNameTable[(int)JobType.JT_SORCERER_B - JOBMINUS] = "¼Ò¼­·¯";
            m_newPcJobNameTable[(int)JobType.JT_MINSTREL_B - JOBMINUS] = "¹Î½ºÆ®·²";
            m_newPcJobNameTable[(int)JobType.JT_WANDERER_B - JOBMINUS] = "¿ø´õ·¯";
            m_newPcJobNameTable[(int)JobType.JT_SURA_B - JOBMINUS] = "½´¶ó";
            m_newPcJobNameTable[(int)JobType.JT_GENETIC_B - JOBMINUS] = "Á¦³×¸¯";
            m_newPcJobNameTable[(int)JobType.JT_SHADOW_CHASER_B - JOBMINUS] = "½¦µµ¿ìÃ¼ÀÌ¼­";

            m_newPcJobNameTable[(int)JobType.JT_RUNE_CHICKEN_B - JOBMINUS] = "·é³ªÀÌÆ®»Ú¶ì";
            m_newPcJobNameTable[(int)JobType.JT_ROYAL_CHICKEN_B - JOBMINUS] = "±×¸®Æù°¡µå";
            m_newPcJobNameTable[(int)JobType.JT_WOLF_RANGER_B - JOBMINUS] = "·¹ÀÎÁ®´Á´ë";
            m_newPcJobNameTable[(int)JobType.JT_MADOGEAR_B - JOBMINUS] = "¸¶µµ±â¾î";

            m_newPcJobNameTable[(int)JobType.JT_FROG_NINJA - JOBMINUS] = "µÎ²¨ºñ´ÑÀÚ";
            m_newPcJobNameTable[(int)JobType.JT_PECO_GUNNER - JOBMINUS] = "ÆäÄÚ°Ç³Ê";
            m_newPcJobNameTable[(int)JobType.JT_PECO_SWORD - JOBMINUS] = "ÆäÄÚ°Ë»ç";
            m_newPcJobNameTable[(int)JobType.JT_FROG_LINKER - JOBMINUS] = "µÎ²¨ºñ¼Ò¿ï¸µÄ¿";
            m_newPcJobNameTable[(int)JobType.JT_PIG_WHITESMITH - JOBMINUS] = "È­ÀÌÆ®½º¹Ì½º¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_PIG_MERCHANT - JOBMINUS] = "»óÀÎ¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_PIG_GENETIC - JOBMINUS] = "Á¦³×¸¯¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_PIG_CREATOR - JOBMINUS] = "Å©¸®¿¡ÀÌÅÍ¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_ARCHER - JOBMINUS] = "Å¸Á¶±Ã¼ö";
            m_newPcJobNameTable[(int)JobType.JT_PORING_STAR - JOBMINUS] = "±Ç¼ºÆ÷¸µ";
            m_newPcJobNameTable[(int)JobType.JT_PORING_NOVICE - JOBMINUS] = "³ëºñ½ºÆ÷¸µ";
            m_newPcJobNameTable[(int)JobType.JT_SHEEP_MONK - JOBMINUS] = "¸ùÅ©¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_SHEEP_ACO - JOBMINUS] = "º¹»ç¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_SHEEP_SURA - JOBMINUS] = "½´¶ó¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_PORING_SNOVICE - JOBMINUS] = "½´ÆÛ³ëºñ½ºÆ÷¸µ";
            m_newPcJobNameTable[(int)JobType.JT_SHEEP_ARCB - JOBMINUS] = "¾ÆÅ©ºñ¼ó¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_FOX_MAGICIAN - JOBMINUS] = "¿©¿ì¸¶¹ý»ç";
            m_newPcJobNameTable[(int)JobType.JT_FOX_SAGE - JOBMINUS] = "¿©¿ì¼¼ÀÌÁö";
            m_newPcJobNameTable[(int)JobType.JT_FOX_SORCERER - JOBMINUS] = "¿©¿ì¼Ò¼­·¯";
            m_newPcJobNameTable[(int)JobType.JT_FOX_WARLOCK - JOBMINUS] = "¿©¿ì¿ö·Ï";
            m_newPcJobNameTable[(int)JobType.JT_FOX_WIZ - JOBMINUS] = "¿©¿ìÀ§Àúµå";
            m_newPcJobNameTable[(int)JobType.JT_FOX_PROF - JOBMINUS] = "¿©¿ìÇÁ·ÎÆä¼­";
            m_newPcJobNameTable[(int)JobType.JT_FOX_HWIZ - JOBMINUS] = "¿©¿ìÇÏÀÌÀ§Àúµå";
            m_newPcJobNameTable[(int)JobType.JT_PIG_ALCHE - JOBMINUS] = "¿¬±Ý¼ú»ç¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_PIG_BLACKSMITH - JOBMINUS] = "Á¦Ã¶°ø¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_SHEEP_CHAMP - JOBMINUS] = "Ã¨ÇÇ¿Â¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_DOG_G_CROSS - JOBMINUS] = "ÄÌº£·Î½º±æ·ÎÆ¾Å©·Î½º";
            m_newPcJobNameTable[(int)JobType.JT_DOG_THIEF - JOBMINUS] = "ÄÌº£·Î½ºµµµÏ";
            m_newPcJobNameTable[(int)JobType.JT_DOG_ROGUE - JOBMINUS] = "ÄÌº£·Î½º·Î±×";
            m_newPcJobNameTable[(int)JobType.JT_DOG_CHASER - JOBMINUS] = "ÄÌº£·Î½º½¦µµ¿ìÃ¼ÀÌ¼­";
            m_newPcJobNameTable[(int)JobType.JT_DOG_STALKER - JOBMINUS] = "ÄÌº£·Î½º½ºÅäÄ¿";
            m_newPcJobNameTable[(int)JobType.JT_DOG_ASSASSIN - JOBMINUS] = "ÄÌº£·Î½º¾î½ê½Å";
            m_newPcJobNameTable[(int)JobType.JT_DOG_ASSA_X - JOBMINUS] = "ÄÌº£·Î½º¾î½ê½ÅÅ©·Î½º";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_DANCER - JOBMINUS] = "Å¸Á¶¹«Èñ";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_MINSTREL - JOBMINUS] = "Å¸Á¶¹Î½ºÆ®·²";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_BARD - JOBMINUS] = "Å¸Á¶¹Ùµå";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_SNIPER - JOBMINUS] = "Å¸Á¶½º³ªÀÌÆÛ";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_WANDER - JOBMINUS] = "Å¸Á¶¿ø´õ·¯";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_ZIPSI - JOBMINUS] = "Å¸Á¶Â¤½Ã";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_CROWN - JOBMINUS] = "Å¸Á¶Å©¶ó¿î";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_HUNTER - JOBMINUS] = "Å¸Á¶ÇåÅÍ";
            m_newPcJobNameTable[(int)JobType.JT_PORING_TAEKWON - JOBMINUS] = "ÅÂ±Ç¼Ò³âÆ÷¸µ";
            m_newPcJobNameTable[(int)JobType.JT_SHEEP_PRIEST - JOBMINUS] = "ÇÁ¸®½ºÆ®¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_SHEEP_HPRIEST - JOBMINUS] = "ÇÏÀÌÇÁ¸®½ºÆ®¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_PORING_NOVICE_B - JOBMINUS] = "³ëºñ½ºÆ÷¸µ";
            m_newPcJobNameTable[(int)JobType.JT_PECO_SWORD_B - JOBMINUS] = "ÆäÄÚ°Ë»ç";
            m_newPcJobNameTable[(int)JobType.JT_FOX_MAGICIAN_B - JOBMINUS] = "¿©¿ì¸¶¹ý»ç";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_ARCHER_B - JOBMINUS] = "Å¸Á¶±Ã¼ö";
            m_newPcJobNameTable[(int)JobType.JT_SHEEP_ACO_B - JOBMINUS] = "º¹»ç¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_PIG_MERCHANT_B - JOBMINUS] = "»óÀÎ¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_HUNTER_B - JOBMINUS] = "Å¸Á¶ÇåÅÍ";
            m_newPcJobNameTable[(int)JobType.JT_DOG_ASSASSIN_B - JOBMINUS] = "ÄÌº£·Î½º¾î½ê½Å";
            m_newPcJobNameTable[(int)JobType.JT_SHEEP_MONK_B - JOBMINUS] = "¸ùÅ©¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_FOX_SAGE_B - JOBMINUS] = "¿©¿ì¼¼ÀÌÁö";
            m_newPcJobNameTable[(int)JobType.JT_DOG_ROGUE_B - JOBMINUS] = "ÄÌº£·Î½º·Î±×";
            m_newPcJobNameTable[(int)JobType.JT_PIG_ALCHE_B - JOBMINUS] = "¿¬±Ý¼ú»ç¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_BARD_B - JOBMINUS] = "Å¸Á¶¹Ùµå";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_DANCER_B - JOBMINUS] = "Å¸Á¶¹«Èñ";
            m_newPcJobNameTable[(int)JobType.JT_PORING_SNOVICE_B - JOBMINUS] = "½´ÆÛ³ëºñ½ºÆ÷¸µ";
            m_newPcJobNameTable[(int)JobType.JT_FOX_WARLOCK_B - JOBMINUS] = "¿©¿ì¿ö·Ï";
            m_newPcJobNameTable[(int)JobType.JT_SHEEP_ARCB_B - JOBMINUS] = "¾ÆÅ©ºñ¼ó¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_DOG_G_CROSS_B - JOBMINUS] = "ÄÌº£·Î½º±æ·ÎÆ¾Å©·Î½º";
            m_newPcJobNameTable[(int)JobType.JT_FOX_SORCERER_B - JOBMINUS] = "¿©¿ì¼Ò¼­·¯";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_MINSTREL_B - JOBMINUS] = "Å¸Á¶¹Î½ºÆ®·²";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_WANDER_B - JOBMINUS] = "Å¸Á¶¿ø´õ·¯";
            m_newPcJobNameTable[(int)JobType.JT_SHEEP_SURA_B - JOBMINUS] = "½´¶ó¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_PIG_GENETIC_B - JOBMINUS] = "Á¦³×¸¯¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_DOG_THIEF_B - JOBMINUS] = "ÄÌº£·Î½ºµµµÏ";
            m_newPcJobNameTable[(int)JobType.JT_DOG_CHASER_B - JOBMINUS] = "ÄÌº£·Î½º½¦µµ¿ìÃ¼ÀÌ¼­";
            m_newPcJobNameTable[(int)JobType.JT_PORING_NOVICE_H - JOBMINUS] = "³ëºñ½ºÆ÷¸µ";
            m_newPcJobNameTable[(int)JobType.JT_PECO_SWORD_H - JOBMINUS] = "ÆäÄÚ°Ë»ç";
            m_newPcJobNameTable[(int)JobType.JT_FOX_MAGICIAN_H - JOBMINUS] = "¿©¿ì¸¶¹ý»ç";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_ARCHER_H - JOBMINUS] = "Å¸Á¶±Ã¼ö";
            m_newPcJobNameTable[(int)JobType.JT_SHEEP_ACO_H - JOBMINUS] = "º¹»ç¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_PIG_MERCHANT_H - JOBMINUS] = "»óÀÎ¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_DOG_THIEF_H - JOBMINUS] = "ÄÌº£·Î½ºµµµÏ";

            m_newPcJobNameTable[(int)JobType.JT_SUPERNOVICE2 - JOBMINUS] = "½´ÆÛ³ëºñ½º";
            m_newPcJobNameTable[(int)JobType.JT_SUPERNOVICE2_B - JOBMINUS] = "½´ÆÛ³ëºñ½º";
            m_newPcJobNameTable[(int)JobType.JT_PORING_SNOVICE2 - JOBMINUS] = "½´ÆÛ³ëºñ½ºÆ÷¸µ";
            m_newPcJobNameTable[(int)JobType.JT_PORING_SNOVICE2_B - JOBMINUS] = "½´ÆÛ³ëºñ½ºÆ÷¸µ";

            m_newPcJobNameTable[(int)JobType.JT_SHEEP_PRIEST_B - JOBMINUS] = "ÇÁ¸®½ºÆ®¾ËÆÄÄ«";
            m_newPcJobNameTable[(int)JobType.JT_FOX_WIZ_B - JOBMINUS] = "¿©¿ìÀ§Àúµå";
            m_newPcJobNameTable[(int)JobType.JT_PIG_BLACKSMITH_B - JOBMINUS] = "Á¦Ã¶°ø¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_PIG_MECHANIC - JOBMINUS] = "¹ÌÄÉ´Ð¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_RANGER - JOBMINUS] = "Å¸Á¶·¹ÀÎÁ®";
            m_newPcJobNameTable[(int)JobType.JT_LION_KNIGHT - JOBMINUS] = "»çÀÚ±â»ç";
            m_newPcJobNameTable[(int)JobType.JT_LION_KNIGHT_H - JOBMINUS] = "»çÀÚ·Îµå³ªÀÌÆ®";
            m_newPcJobNameTable[(int)JobType.JT_LION_ROYAL_GUARD - JOBMINUS] = "»çÀÚ·Î¾â°¡µå";
            m_newPcJobNameTable[(int)JobType.JT_LION_RUNE_KNIGHT - JOBMINUS] = "»çÀÚ·é³ªÀÌÆ®";
            m_newPcJobNameTable[(int)JobType.JT_LION_CRUSADER - JOBMINUS] = "»çÀÚÅ©·ç¼¼ÀÌ´õ";
            m_newPcJobNameTable[(int)JobType.JT_LION_CRUSADER_H - JOBMINUS] = "»çÀÚÆÈ¶óµò";
            m_newPcJobNameTable[(int)JobType.JT_PIG_MECHANIC_B - JOBMINUS] = "¹ÌÄÉ´Ð¸äµÅÁö";
            m_newPcJobNameTable[(int)JobType.JT_OSTRICH_RANGER_B - JOBMINUS] = "Å¸Á¶·¹ÀÎÁ®";
            m_newPcJobNameTable[(int)JobType.JT_LION_KNIGHT_B - JOBMINUS] = "»çÀÚ±â»ç";
            m_newPcJobNameTable[(int)JobType.JT_LION_ROYAL_GUARD_B - JOBMINUS] = "»çÀÚ·Î¾â°¡µå";
            m_newPcJobNameTable[(int)JobType.JT_LION_RUNE_KNIGHT_B - JOBMINUS] = "»çÀÚ·é³ªÀÌÆ®";
            m_newPcJobNameTable[(int)JobType.JT_LION_CRUSADER_B - JOBMINUS] = "»çÀÚÅ©·ç¼¼ÀÌ´õ";

            m_newPcJobNameTable[(int)JobType.JT_KAGEROU - JOBMINUS] = "kagerou";
            m_newPcJobNameTable[(int)JobType.JT_OBORO - JOBMINUS] = "oboro";
            m_newPcJobNameTable[(int)JobType.JT_FROG_KAGEROU - JOBMINUS] = "frog_kagerou";
            m_newPcJobNameTable[(int)JobType.JT_FROG_OBORO - JOBMINUS] = "frog_oboro";
            m_newPcJobNameTable[(int)JobType.JT_REBELLION - JOBMINUS] = "rebellion";

            m_newPcJobNameTable[4216 - JOBMINUS] = "peco_rebellion";

            m_newPcJobNameTable[(int)JobType.JT_DO_SUMMONER - JOBMINUS] = "summoner";
            m_newPcJobNameTable[(int)JobType.JT_DO_CART_SUMMONER - JOBMINUS] = "cart_summoner";
            m_newPcJobNameTable[(int)JobType.JT_DO_SUMMONER_B - JOBMINUS] = "summoner";
            m_newPcJobNameTable[(int)JobType.JT_DO_CART_SUMMONER_B - JOBMINUS] = "cart_summoner";

            m_newPcJobNameTable[(int)JobType.JT_NINJA_B - JOBMINUS] = "´ÑÀÚ";
            m_newPcJobNameTable[(int)JobType.JT_KAGEROU_B - JOBMINUS] = "kagerou";
            m_newPcJobNameTable[(int)JobType.JT_OBORO_B - JOBMINUS] = "oboro";
            m_newPcJobNameTable[(int)JobType.JT_TAEKWON_B - JOBMINUS] = "ÅÂ±Ç¼Ò³â";
            m_newPcJobNameTable[(int)JobType.JT_STAR_B - JOBMINUS] = "±Ç¼º";
            m_newPcJobNameTable[(int)JobType.JT_LINKER_B - JOBMINUS] = "¼Ò¿ï¸µÄ¿";
            m_newPcJobNameTable[(int)JobType.JT_GUNSLINGER_B - JOBMINUS] = "°Ç³Ê";
            m_newPcJobNameTable[(int)JobType.JT_REBELLION_B - JOBMINUS] = "rebellion";

            m_newPcJobNameTable[(int)JobType.JT_FROG_NINJA_B - JOBMINUS] = "µÎ²¨ºñ´ÑÀÚ";
            m_newPcJobNameTable[(int)JobType.JT_FROG_KAGEROU_B - JOBMINUS] = "frog_kagerou";
            m_newPcJobNameTable[(int)JobType.JT_FROG_OBORO_B - JOBMINUS] = "frog_oboro";
            m_newPcJobNameTable[(int)JobType.JT_PORING_TAEKWON_B - JOBMINUS] = "ÅÂ±Ç¼Ò³âÆ÷¸µ";
            m_newPcJobNameTable[(int)JobType.JT_PORING_STAR_B - JOBMINUS] = "±Ç¼ºÆ÷¸µ";
            m_newPcJobNameTable[(int)JobType.JT_FROG_LINKER_B - JOBMINUS] = "µÎ²¨ºñ¼Ò¿ï¸µÄ¿";
            m_newPcJobNameTable[(int)JobType.JT_PECO_GUNSLINGER_B - JOBMINUS] = "ÆäÄÚ°Ç³Ê";
            m_newPcJobNameTable[(int)JobType.JT_PECO_REBELLION_B - JOBMINUS] = "peco_rebellion";
            m_newPcJobNameTable[(int)JobType.JT_STAR2_B - JOBMINUS] = "±Ç¼ºÀ¶ÇÕ";

            m_newPcJobNameTable[(int)JobType.JT_STAR_EMPEROR - JOBMINUS] = "¼ºÁ¦";
            m_newPcJobNameTable[(int)JobType.JT_SOUL_REAPER - JOBMINUS] = "¼Ò¿ï¸®ÆÛ";
            m_newPcJobNameTable[(int)JobType.JT_STAR_EMPEROR_B - JOBMINUS] = "¼ºÁ¦";
            m_newPcJobNameTable[(int)JobType.JT_SOUL_REAPER_B - JOBMINUS] = "¼Ò¿ï¸®ÆÛ";
            m_newPcJobNameTable[(int)JobType.JT_STAR2_EMPEROR - JOBMINUS] = "¼ºÁ¦À¶ÇÕ";
            m_newPcJobNameTable[(int)JobType.JT_STAR2_EMPEROR_B - JOBMINUS] = "¼ºÁ¦À¶ÇÕ";
            m_newPcJobNameTable[(int)JobType.JT_HAETAE_STAR_EMPEROR - JOBMINUS] = "ÇØÅÂ¼ºÁ¦";
            m_newPcJobNameTable[(int)JobType.JT_HAETAE_SOUL_REAPER - JOBMINUS] = "ÇØÅÂ¼Ò¿ï¸®ÆÛ";
            m_newPcJobNameTable[(int)JobType.JT_HAETAE_STAR_EMPEROR_B - JOBMINUS] = "ÇØÅÂ¼ºÁ¦";
            m_newPcJobNameTable[(int)JobType.JT_HAETAE_SOUL_REAPER_B - JOBMINUS] = "ÇØÅÂ¼Ò¿ï¸®ÆÛ";

            m_newPcJobNameTable[(int)JobType.JT_DRAGON_KNIGHT - JOBMINUS] = "DRAGON_KNIGHT";
            m_newPcJobNameTable[(int)JobType.JT_MEISTER - JOBMINUS] = "MEISTER";
            m_newPcJobNameTable[(int)JobType.JT_SHADOW_CROSS - JOBMINUS] = "SHADOW_CROSS";
            m_newPcJobNameTable[(int)JobType.JT_ARCH_MAGE - JOBMINUS] = "ARCH_MAGE";
            m_newPcJobNameTable[(int)JobType.JT_CARDINAL - JOBMINUS] = "CARDINAL";
            m_newPcJobNameTable[(int)JobType.JT_WINDHAWK - JOBMINUS] = "WINDHAWK";
            m_newPcJobNameTable[(int)JobType.JT_IMPERIAL_GUARD - JOBMINUS] = "IMPERIAL_GUARD";
            m_newPcJobNameTable[(int)JobType.JT_BIOLO - JOBMINUS] = "BIOLO";
            m_newPcJobNameTable[(int)JobType.JT_ABYSS_CHASER - JOBMINUS] = "ABYSS_CHASER";
            m_newPcJobNameTable[(int)JobType.JT_ELEMENTAL_MASTER - JOBMINUS] = "ELEMETAL_MASTER";
            m_newPcJobNameTable[(int)JobType.JT_INQUISITOR - JOBMINUS] = "INQUISITOR";
            m_newPcJobNameTable[(int)JobType.JT_TROUBADOUR - JOBMINUS] = "TROUBADOUR";
            m_newPcJobNameTable[(int)JobType.JT_TROUVERE - JOBMINUS] = "TROUVERE";

            m_newPcJobNameTable[(int)JobType.JT_DRAGON_KNIGHT_RIDING - JOBMINUS] = "DRAGON_KNIGHT_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_MEISTER_RIDING - JOBMINUS] = "MEISTER_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_SHADOW_CROSS_RIDING - JOBMINUS] = "SHADOW_CROSS_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_ARCH_MAGE_RIDING - JOBMINUS] = "ARCH_MAGE_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_CARDINAL_RIDING - JOBMINUS] = "CARDINAL_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_WINDHAWK_RIDING - JOBMINUS] = "WINDHAWK_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_IMPERIAL_GUARD_RIDING - JOBMINUS] = "IMPERIAL_GUARD_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_BIOLO_RIDING - JOBMINUS] = "BIOLO_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_ABYSS_CHASER_RIDING - JOBMINUS] = "ABYSS_CHASER_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_ELEMENTAL_MASTER_RIDING - JOBMINUS] = "ELEMETAL_MASTER_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_INQUISITOR_RIDING - JOBMINUS] = "INQUISITOR_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_TROUBADOUR_RIDING - JOBMINUS] = "TROUBADOUR_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_TROUVERE_RIDING - JOBMINUS] = "TROUVERE_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_WOLF_WINDHAWK - JOBMINUS] = "WOLF_WINDHAWK";
            m_newPcJobNameTable[(int)JobType.JT_MEISTER_MADOGEAR - JOBMINUS] = "MEISTER_MADOGEAR1";
            m_newPcJobNameTable[(int)JobType.JT_DRAGON_KNIGHT_CHICKEN - JOBMINUS] = "DRAGON_KNIGHT_CHICKEN";
            m_newPcJobNameTable[(int)JobType.JT_IMPERIAL_GUARD_CHICKEN - JOBMINUS] = "IMPERIAL_GUARD_CHICKEN";

            m_newPcJobNameTable[(int)JobType.JT_SKY_EMPEROR - JOBMINUS] = "SKY_EMPEROR";
            m_newPcJobNameTable[(int)JobType.JT_SOUL_ASCETIC - JOBMINUS] = "SOUL_ASCETIC";
            m_newPcJobNameTable[(int)JobType.JT_SHINKIRO - JOBMINUS] = "SHINKIRO";
            m_newPcJobNameTable[(int)JobType.JT_SHIRANUI - JOBMINUS] = "SHIRANUI";
            m_newPcJobNameTable[(int)JobType.JT_NIGHT_WATCH - JOBMINUS] = "NIGHT_WATCH";
            m_newPcJobNameTable[(int)JobType.JT_HYPER_NOVICE - JOBMINUS] = "HYPER_NOVICE";
            m_newPcJobNameTable[(int)JobType.JT_SPIRIT_HANDLER - JOBMINUS] = "SPIRIT_HANDLER";
            m_newPcJobNameTable[(int)JobType.JT_SKY_EMPEROR_RIDING - JOBMINUS] = "SKY_EMPEROR_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_SOUL_ASCETIC_RIDING - JOBMINUS] = "SOUL_ASCETIC_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_SHINKIRO_RIDING - JOBMINUS] = "SHINKIRO_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_SHIRANUI_RIDING - JOBMINUS] = "SHIRANUI_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_NIGHT_WATCH_RIDING - JOBMINUS] = "NIGHT_WATCH_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_HYPER_NOVICE_RIDING - JOBMINUS] = "HYPER_NOVICE_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_SPIRIT_HANDLER_RIDING - JOBMINUS] = "SPIRIT_HANDLER_RIDING";
            m_newPcJobNameTable[(int)JobType.JT_SKY_EMPEROR2 - JOBMINUS] = "SKY_EMPEROR2";
        }
    }
}