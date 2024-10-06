using System;
using UnityEngine;

#if UNITY_EDITOR
namespace UnityRO.Net
{
    [Serializable]
    public class RecordedNetworkPacket
    {
        public float Time;
        public byte[] Data;
        public bool IsOut;
    }

    public class RecordedNetworkTraffic : ScriptableObject
    {
        [SerializeField] public RecordedNetworkPacket[] Packets;
    }
}
#endif