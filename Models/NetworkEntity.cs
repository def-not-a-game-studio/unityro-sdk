public class NetworkEntity {
    public int EntityType { get; private set; }
    public int GID { get; private set; }
    public string Name { get; private set; }

    public NetworkEntity(int type, int GID, string name) {
        this.EntityType = type;
        this.GID = GID;
        this.Name = name;
    }
}