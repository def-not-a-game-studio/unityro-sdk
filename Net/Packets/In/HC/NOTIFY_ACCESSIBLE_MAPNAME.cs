using System.Collections.Generic;
using ROIO.Utils;
using UnityEngine;

public partial class HC {

    [PacketHandler(HEADER, "HC_NOTIFY_ACCESSIBLE_MAPNAME")]
    public class NOTIFY_ACCESSIBLE_MAPNAME : InPacket {

        public const PacketHeader HEADER = PacketHeader.HC_NOTIFY_ACCESSIBLE_MAPNAME;
        public PacketHeader Header => HEADER;

        public List<MapInfo> mapinfoList = new();

        public void Read(MemoryStreamReader br, int size) {
            var count = (br.Length - br.Position) / 20;

            for(var i = 0; i < count; i++) {
                var status = br.ReadInt();
                var mapname = br.ReadBinaryString(16);
                Debug.Log($"MapEntry {status} {mapname}");
                mapinfoList.Add(new MapInfo {
                    status = status,
                    mapName = mapname
                });
            }
        }

        public struct MapInfo {
            public int status;
            public string mapName;
        }
    }
}