using ROIO.Utils;

public partial class ZC {
    
    [PacketHandler(HEADER, "ZC_PING_LIVE", SIZE)]
    public class PING_LIVE : InPacket {
        public const PacketHeader HEADER = PacketHeader.ZC_PING_LIVE;
        public const int SIZE = 2;
        public PacketHeader Header => HEADER;
        
        public void Read(MemoryStreamReader br, int size) { }
    }
}