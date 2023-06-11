using ROIO.Utils;

public partial class ZC {
    [PacketHandler(HEADER, "ZC_ACK_REQNAMEALL_NPC", SIZE)]
    public class ACK_REQNAMEALL_NPC : InPacket {
        
        public const PacketHeader HEADER = PacketHeader.ZC_ACK_REQNAMEALL_NPC;
        public PacketHeader Header => HEADER;
        public const int SIZE = 58;
        public void Read(MemoryStreamReader br, int size) {
            
        }

    }
}