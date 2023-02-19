public interface INetworkEntity
{
    EntityType GetEntityType();
    int GetEntityGID();
    string GetEntityName();
    void UpdateSprites();
}
