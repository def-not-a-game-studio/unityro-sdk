using System;
using UnityEngine;

namespace UnityRO.Net.Editor {

    [Serializable]
    public class RecordedNetworkPacket {
        public float Time;
        public byte[] Data;
        public bool IsOut;
    }
    
    public class RecordedNetworkTraffic : ScriptableObject {
        [SerializeField] public RecordedNetworkPacket[] Packets;
    }
}