using ROIO.Utils;

public partial class ZC {

    [PacketHandler(HEADER, "ZC_PERSONAL_INFOMATION_CHN")]
    public class PERSONAL_INFOMATION_CHN : InPacket {

        public const PacketHeader HEADER = PacketHeader.ZC_PERSONAL_INFOMATION_CHN;
        public PacketHeader Header => HEADER;
        public void Read(MemoryStreamReader br, int size) {
        }
    }
}
