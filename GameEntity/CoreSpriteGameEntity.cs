public abstract class CoreSpriteGameEntity : CoreGameEntity {
    
    public abstract int Direction { get; }
    public abstract int HeadDirection { get; }
    public abstract bool IsMonster { get; }

}