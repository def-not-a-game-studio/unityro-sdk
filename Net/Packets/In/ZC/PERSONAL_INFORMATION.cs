using ROIO.Utils;

public partial class ZC {

    [PacketHandler(HEADER, "ZC_PERSONAL_INFOMATION")]
    public class PERSONAL_INFOMATION : InPacket {

        public const PacketHeader HEADER = PacketHeader.ZC_PERSONAL_INFOMATION;
        public PacketHeader Header => HEADER;
        public void Read(MemoryStreamReader br, int size) {
        }
    }
}
