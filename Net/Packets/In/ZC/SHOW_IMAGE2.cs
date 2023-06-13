using ROIO.Utils;

public partial class ZC {
    
    [PacketHandler(HEADER, "ZC_STATE_CHANGE3", SIZE)]
    public class SHOW_IMAGE2 : InPacket {
        public const PacketHeader HEADER = PacketHeader.ZC_SHOW_IMAGE2;
        public const int SIZE = 67;
        public PacketHeader Header => HEADER;
        
        public string image;
        public int type;

        public void Read(MemoryStreamReader br, int size) {
            image = br.ReadBinaryString(64);
            type = br.ReadByte();
        }
    }
}