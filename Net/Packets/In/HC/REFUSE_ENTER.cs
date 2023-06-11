using ROIO.Utils;

public partial class HC {
    
    [PacketHandler(HEADER, "HC_REFUSE_ENTER", SIZE)]
    public class REFUSE_ENTER : InPacket {
        
        public const PacketHeader HEADER = PacketHeader.HC_REFUSE_ENTER;
        public const int SIZE = 3;

        public int Reason;
        
        public void Read(MemoryStreamReader br, int size) {
            Reason = br.ReadByte();
        }

        public PacketHeader Header => HEADER;
    }
}