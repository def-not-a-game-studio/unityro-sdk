using System;
using System.Linq;
using UnityEngine;
using UnityRO.Core.Database.Emotion;

namespace UnityRO.Core.Database {
    public class DatabaseManager : MonoBehaviour {
        [SerializeField] private JobDatabase JobDb;
        [SerializeField] private SpriteHeadDatabase HeadDb;

        private EmotionDatabase EmotionDb = new();

        public Job GetJobById(int id) {
            return JobDb.Values.FirstOrDefault(it => it.JobId == id) ?? throw new Exception($"Job not found {id}");
        }

        public SpriteHead GetHeadById(int id) {
            return HeadDb.Values.FirstOrDefault(it => it.Id == id) ?? HeadDb.Values.First();
        }

        public int GetEmotionIndex(EmotionType type) {
            return EmotionDb.GetEmotionIndex(type);
        }
    }
}