using ROIO.Utils;

public partial class ZC {
    
    [PacketHandler(HEADER, "ZC_CHANGE_DIRECTION", SIZE)]
    public class CHANGE_DIRECTION : InPacket {
        public const PacketHeader HEADER = PacketHeader.ZC_CHANGE_DIRECTION;
        public const int SIZE = 9;
        public PacketHeader Header => HEADER;
        
        public void Read(MemoryStreamReader br, int size) { }
    }
}