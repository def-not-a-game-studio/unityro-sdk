using ROIO.Utils;

public partial class ZC {
    [PacketHandler(HEADER, "ZC_NOTIFY_HP_TO_GROUPM_R2", SIZE)]
    public class NOTIFY_HP_TO_GROUPM_R2 : InPacket {
        public const PacketHeader HEADER = PacketHeader.ZC_NOTIFY_HP_TO_GROUPM_R2;
        public const int SIZE = 14;
        public PacketHeader Header => HEADER;

        public void Read(MemoryStreamReader br, int size) {
            
        }
    }
}