using ROIO.Utils;

public partial class ZC {

    [PacketHandler(HEADER, "ZC_PERSONAL_INFOMATION2")]
    public class PERSONAL_INFOMATION2 : InPacket {

        public const PacketHeader HEADER = PacketHeader.ZC_PERSONAL_INFOMATION2;
        public PacketHeader Header => HEADER;
        public void Read(MemoryStreamReader br, int size) {
        }
    }
}
