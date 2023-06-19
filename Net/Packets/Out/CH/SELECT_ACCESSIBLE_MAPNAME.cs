public partial class CH {

    public class SELECT_ACCESSIBLE_MAPNAME : OutPacket {

        private const PacketHeader HEADER = PacketHeader.CH_SELECT_ACCESSIBLE_MAPNAME;
        private const int SIZE = 4;

        public int slot;
        public int mapNumber;

        public SELECT_ACCESSIBLE_MAPNAME() : base(HEADER, SIZE){
        }

        public override void Send() {
            Write((byte)slot);
            Write((byte)mapNumber);

            base.Send();
        }
    }
}
