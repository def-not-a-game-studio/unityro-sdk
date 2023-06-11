using ROIO.Utils;

public partial class ZC {
    [PacketHandler(HEADER, "ZC_NPC_CHAT", SIZE)]
    public class NPC_CHAT : InPacket {
        
        public const PacketHeader HEADER = PacketHeader.ZC_NPC_CHAT;
        public PacketHeader Header => HEADER;
        public const int SIZE = -1;
        public void Read(MemoryStreamReader br, int size) {
            
        }
    }
}