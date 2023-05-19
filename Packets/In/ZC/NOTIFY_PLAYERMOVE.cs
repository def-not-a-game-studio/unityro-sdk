﻿using ROIO.Utils;

public partial class ZC {

    [PacketHandler(HEADER, "ZC_NOTIFY_PLAYERMOVE", SIZE)]
    public class NOTIFY_PLAYERMOVE : InPacket {

        public const PacketHeader HEADER = PacketHeader.ZC_NOTIFY_PLAYERMOVE;
        public PacketHeader Header => HEADER;
        public const int SIZE = 12;

        public int[] StartPosition;
        public int[] EndPosition;
        public ulong movementTick;

        public PacketHeader GetHeader() {
            return HEADER;
        }

        public void Read(MemoryStreamReader br, int size) {
            movementTick = br.ReadUInt();
            var moveData = br.ReadPos2();
            StartPosition = new int[2] { moveData[0], moveData[1] };
            EndPosition = new int[2] { moveData[2], moveData[3] };
        }
    }
}
