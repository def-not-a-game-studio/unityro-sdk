using System.Collections.Generic;
using UnityEngine;

namespace UnityRO.Core.Database {
    
    [CreateAssetMenu(menuName = "Heimdallr/Database Entry/Mesh Job")]
    public class MeshJob : Job {
        public CoreMeshGameEntity Female;
        public CoreMeshGameEntity Male;
        public List<Material> ColorsMale;
        public List<Material> ColorsFemale;
    }
}