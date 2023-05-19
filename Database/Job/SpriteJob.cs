using UnityEngine;

namespace UnityRO.Core.Database {
    
    [CreateAssetMenu(menuName = "Heimdallr/Database Entry/Sprite Job")]
    public class SpriteJob : Job {
        public SpriteData Male;
        public SpriteData Female;
    }
}