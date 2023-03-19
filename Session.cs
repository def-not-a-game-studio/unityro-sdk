using System;

public class Session {

    public const int PC_ENTITY_TYPE = 0;

    public static Session CurrentSession { get; private set; }
    public static Action<string> OnMapChanged;

    public int AccountID;
    public NetworkEntity Entity { get; private set; }
    public string CurrentMap { get; private set; }

    public Session(NetworkEntity entity, int accountID) {
        if (entity.EntityType != PC_ENTITY_TYPE) {
            throw new ArgumentException("Cannot start session with non player entity");
        }

        AccountID = accountID;
        this.Entity = entity;
    }

    public void SetCurrentMap(string mapname) {
        CurrentMap = mapname;
        OnMapChanged?.Invoke(mapname);
    }

    public static void StartSession(Session session) {
        CurrentSession = session;
    }
}