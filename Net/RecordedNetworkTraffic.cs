using System;
using UnityEngine;

namespace UnityRO.Net.Editor {

    [Serializable]
    public class RecordedNetworkPacket {
        public byte[] Data;
    }
    
    public class RecordedNetworkTraffic : ScriptableObject {
        [SerializeField] public RecordedNetworkPacket[] Packets;
    }
}