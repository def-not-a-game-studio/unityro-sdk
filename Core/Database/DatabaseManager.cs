using System;
using System.Linq;
using UnityEngine;

namespace UnityRO.Core.Database {
    public class DatabaseManager : MonoBehaviour {
        [SerializeField] private JobDatabase JobDb;
        [SerializeField] private SpriteHeadDatabase HeadDb;

        public Job GetJobById(int id) {
            return JobDb.Values.FirstOrDefault(it => it.JobId == id) ?? throw new Exception($"Job not found {id}");
        }

        public SpriteHead GetHeadById(int id) {
            return HeadDb.Values.FirstOrDefault(it => it.Id == id) ?? HeadDb.Values.First();
        }
    }
}