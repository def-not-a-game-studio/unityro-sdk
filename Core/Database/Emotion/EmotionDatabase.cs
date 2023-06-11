using System.Collections.Generic;

namespace UnityRO.Core.Database.Emotion {
    public class EmotionDatabase {
        private Dictionary<EmotionType, int> EmotionIndexes = new();

        public EmotionDatabase() {
            InitIndexes();
        }

        private void InitIndexes() {
            InsertEmotionListTable(EmotionType.ET_SURPRISE, EmotionType.MSI_EMOTION_SURPRISE, 0);
            InsertEmotionListTable(EmotionType.ET_QUESTION, EmotionType.MSI_EMOTION_QUESTION, 1);
            InsertEmotionListTable(EmotionType.ET_DELIGHT, EmotionType.MSI_EMOTION_DELIGHT, 2);
            InsertEmotionListTable(EmotionType.ET_THROB, EmotionType.MSI_EMOTION_THROB, 3);
            InsertEmotionListTable(EmotionType.ET_BIGTHROB, EmotionType.MSI_EMOTION_BIGTHROB, 4);
            InsertEmotionListTable(EmotionType.ET_SWEAT, EmotionType.MSI_EMOTION_SWEAT, 5);
            InsertEmotionListTable(EmotionType.ET_AHA, EmotionType.MSI_EMOTION_AHA, 6);
            InsertEmotionListTable(EmotionType.ET_FRET, EmotionType.MSI_EMOTION_FRET, 7);
            InsertEmotionListTable(EmotionType.ET_ANGER, EmotionType.MSI_EMOTION_ANGER, 8);
            InsertEmotionListTable(EmotionType.ET_MONEY, EmotionType.MSI_EMOTION_MONEY, 9);
            InsertEmotionListTable(EmotionType.ET_THINK, EmotionType.MSI_EMOTION_THINK, 10);
            InsertEmotionListTable(EmotionType.ET_THANKS, EmotionType.MSI_EMOTION_THANKS, 15);
            InsertEmotionListTable(EmotionType.ET_KEK, EmotionType.MSI_EMOTION_KEK, 16);
            InsertEmotionListTable(EmotionType.ET_SORRY, EmotionType.MSI_EMOTION_SORRY, 17);
            InsertEmotionListTable(EmotionType.ET_SMILE, EmotionType.MSI_EMOTION_SMILE, 18);
            InsertEmotionListTable(EmotionType.ET_PROFUSELY_SWEAT, EmotionType.MSI_EMOTION_PROFUSELY_SWEAT, 19);
            InsertEmotionListTable(EmotionType.ET_SCRATCH, EmotionType.MSI_EMOTION_SCRATCH, 20);
            InsertEmotionListTable(EmotionType.ET_BEST, EmotionType.MSI_EMOTION_BEST, 21);
            InsertEmotionListTable(EmotionType.ET_STARE_ABOUT, EmotionType.MSI_EMOTION_STARE_ABOUT, 22);
            InsertEmotionListTable(EmotionType.ET_HUK, EmotionType.MSI_EMOTION_HUK, 23);
            InsertEmotionListTable(EmotionType.ET_O, EmotionType.MSI_EMOTION_O, 24);
            InsertEmotionListTable(EmotionType.ET_X, EmotionType.MSI_EMOTION_X, 25);
            InsertEmotionListTable(EmotionType.ET_HELP, EmotionType.MSI_EMOTION_HELP, 26);
            InsertEmotionListTable(EmotionType.ET_GO, EmotionType.MSI_EMOTION_GO, 27);
            InsertEmotionListTable(EmotionType.ET_CRY, EmotionType.MSI_EMOTION_CRY, 28);
            InsertEmotionListTable(EmotionType.ET_KIK, EmotionType.MSI_EMOTION_KIK, 29);
            InsertEmotionListTable(EmotionType.ET_CHUP, EmotionType.MSI_EMOTION_CHUP, 30);
            InsertEmotionListTable(EmotionType.ET_CHUPCHUP, EmotionType.MSI_EMOTION_CHUPCHUP, 31);
            InsertEmotionListTable(EmotionType.ET_HNG, EmotionType.MSI_EMOTION_HNG, 32);
            InsertEmotionListTable(EmotionType.ET_OK, EmotionType.MSI_EMOTION_OK, 33);
            InsertEmotionListTable(EmotionType.ET_STARE, EmotionType.MSI_EMOTION_STARE, 35);
            InsertEmotionListTable(EmotionType.ET_HUNGRY, EmotionType.MSI_EMOTION_HUNGRY, 36);
            InsertEmotionListTable(EmotionType.ET_COOL, EmotionType.MSI_EMOTION_COOL, 37);
            InsertEmotionListTable(EmotionType.ET_MERONG, EmotionType.MSI_EMOTION_MERONG, 38);
            InsertEmotionListTable(EmotionType.ET_SHY, EmotionType.MSI_EMOTION_SHY, 39);
            InsertEmotionListTable(EmotionType.ET_GOODBOY, EmotionType.MSI_EMOTION_GOODBOY, 40);
            InsertEmotionListTable(EmotionType.ET_SPTIME, EmotionType.MSI_EMOTION_SPTIME, 41);
            InsertEmotionListTable(EmotionType.ET_SEXY, EmotionType.MSI_EMOTION_SEXY, 42);
            InsertEmotionListTable(EmotionType.ET_COMEON, EmotionType.MSI_EMOTION_COMEON, 43);
            InsertEmotionListTable(EmotionType.ET_SLEEPY, EmotionType.MSI_EMOTION_SLEEPY, 44);
            InsertEmotionListTable(EmotionType.ET_CONGRATULATION, EmotionType.MSI_EMOTION_CONGRATULATION, 45);
            InsertEmotionListTable(EmotionType.ET_HPTIME, EmotionType.MSI_EMOTION_HPTIME, 46);
            InsertEmotionListTable(EmotionType.ET_SPARK, EmotionType.MSI_EMOTION_SPARK, 51);
            InsertEmotionListTable(EmotionType.ET_CONFUSE, EmotionType.MSI_EMOTION_CONFUSE, 52);
            InsertEmotionListTable(EmotionType.ET_OHNO, EmotionType.MSI_EMOTION_OHNO, 53);
            InsertEmotionListTable(EmotionType.ET_HUM, EmotionType.MSI_EMOTION_HUM, 54);
            InsertEmotionListTable(EmotionType.ET_BLABLA, EmotionType.MSI_EMOTION_BLABLA, 55);
            InsertEmotionListTable(EmotionType.ET_OTL, EmotionType.MSI_EMOTION_OTL, 56);
            InsertEmotionListTable(EmotionType.ET_ROCK, EmotionType.MSI_EMOTION_ROCK, 11);
            InsertEmotionListTable(EmotionType.ET_SCISSOR, EmotionType.MSI_EMOTION_SCISSOR, 12);
            InsertEmotionListTable(EmotionType.ET_WRAP, EmotionType.MSI_EMOTION_WRAP, 13);
            InsertEmotionListTable(EmotionType.ET_LUV, EmotionType.MSI_EMOTION_LUV, 64);
            InsertEmotionListTable(EmotionType.ET_MOBILE, EmotionType.MSI_EMOTION_MOBILE, 67);
            InsertEmotionListTable(EmotionType.ET_MAIL, EmotionType.MSI_EMOTION_MAIL, 68);
            InsertEmotionListTable(EmotionType.ET_ANTENNA0, EmotionType.MSI_EMOTION_ANTENNA0, 69);
            InsertEmotionListTable(EmotionType.ET_ANTENNA1, EmotionType.MSI_EMOTION_ANTENNA1, 70);
            InsertEmotionListTable(EmotionType.ET_ANTENNA2, EmotionType.MSI_EMOTION_ANTENNA2, 71);
            InsertEmotionListTable(EmotionType.ET_ANTENNA3, EmotionType.MSI_EMOTION_ANTENNA3, 72);
            InsertEmotionListTable(EmotionType.ET_HUM2, EmotionType.MSI_EMOTION_HUM2, 73);
            InsertEmotionListTable(EmotionType.ET_ABS, EmotionType.MSI_EMOTION_ABS, 74);
            InsertEmotionListTable(EmotionType.ET_OOPS, EmotionType.MSI_EMOTION_OOPS, 75);
            InsertEmotionListTable(EmotionType.ET_SPIT, EmotionType.MSI_EMOTION_SPIT, 76);
            InsertEmotionListTable(EmotionType.ET_ENE, EmotionType.MSI_EMOTION_ENE, 77);
            InsertEmotionListTable(EmotionType.ET_PANIC, EmotionType.MSI_EMOTION_PANIC, 78);
            InsertEmotionListTable(EmotionType.ET_WHISP, EmotionType.MSI_EMOTION_WHISP, 79);
            InsertEmotionListTable(EmotionType.ET_FLAG, EmotionType.NONE, 14);
            InsertEmotionListTable(EmotionType.ET_CHAT_PROHIBIT, EmotionType.NONE, 1000);
            InsertEmotionListTable(EmotionType.ET_INDONESIA_FLAG, EmotionType.NONE, 34);
            InsertEmotionListTable(EmotionType.ET_PH_FLAG, EmotionType.NONE, 47);
            InsertEmotionListTable(EmotionType.ET_MY_FLAG, EmotionType.NONE, 48);
            InsertEmotionListTable(EmotionType.ET_SI_FLAG, EmotionType.NONE, 49);
            InsertEmotionListTable(EmotionType.ET_BR_FLAG, EmotionType.NONE, 50);
            InsertEmotionListTable(EmotionType.ET_DICE1, EmotionType.NONE, 57);
            InsertEmotionListTable(EmotionType.ET_DICE2, EmotionType.NONE, 58);
            InsertEmotionListTable(EmotionType.ET_DICE3, EmotionType.NONE, 59);
            InsertEmotionListTable(EmotionType.ET_DICE4, EmotionType.NONE, 60);
            InsertEmotionListTable(EmotionType.ET_DICE5, EmotionType.NONE, 61);
            InsertEmotionListTable(EmotionType.ET_DICE6, EmotionType.NONE, 62);
            InsertEmotionListTable(EmotionType.ET_INDIA_FLAG, EmotionType.NONE, 63);
            InsertEmotionListTable(EmotionType.ET_FLAG8, EmotionType.NONE, 65);
            InsertEmotionListTable(EmotionType.ET_FLAG9, EmotionType.NONE, 66);
            InsertEmotionListTable(EmotionType.ET_YUT1, EmotionType.NONE, 86);
            InsertEmotionListTable(EmotionType.ET_YUT2, EmotionType.NONE, 87);
            InsertEmotionListTable(EmotionType.ET_YUT3, EmotionType.NONE, 88);
            InsertEmotionListTable(EmotionType.ET_YUT4, EmotionType.NONE, 89);
            InsertEmotionListTable(EmotionType.ET_YUT5, EmotionType.NONE, 90);
            InsertEmotionListTable(EmotionType.ET_YUT6, EmotionType.NONE, 91);
            InsertEmotionListTable(EmotionType.ET_YUT7, EmotionType.NONE, 92);
        }

        private void InsertEmotionListTable(EmotionType a, EmotionType b, int actionIndex) {
            EmotionIndexes[a] = actionIndex;
        }

        public int GetEmotionIndex(EmotionType emotionType) {
            return EmotionIndexes.TryGetValue(emotionType, out var index) ? index : 0;
        }
    }
}