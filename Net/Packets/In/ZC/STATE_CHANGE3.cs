using ROIO.Utils;

public partial class ZC {
    
    [PacketHandler(HEADER, "ZC_STATE_CHANGE3", SIZE)]
    public class STATE_CHANGE3 : InPacket {
        public const PacketHeader HEADER = PacketHeader.ZC_STATE_CHANGE3;
        public const int SIZE = 15;
        public PacketHeader Header => HEADER;
        
        public uint AID;
        public short bodyState;
        public short healthState;
        public int effectState;
        public int isPKModeON;

        public void Read(MemoryStreamReader br, int size) {
            AID = br.ReadUInt();
            bodyState = br.ReadShort();
            healthState = br.ReadShort();
            effectState = br.ReadInt();
            isPKModeON = br.ReadByte();
        }
    }
}