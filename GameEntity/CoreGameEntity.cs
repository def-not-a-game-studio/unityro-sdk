using UnityEngine;
using UnityRO.Core;

public abstract class CoreGameEntity : ManagedMonoBehaviour {
    
    public abstract GameEntityBaseStatus Status { get; }

    public abstract void ChangeMotion(MotionRequest request);

    public abstract void Init(GameEntityBaseStatus gameEntityBaseStatus);

    public abstract bool HasAuthority();
    public abstract int GetEntityGID();
    
    /// <summary>
    /// Use this when you have an offset to add to the current position to walk to
    /// </summary>
    /// <param name="destination"></param>
    public abstract void RequestOffsetMovement(Vector2 destination);
    
    /// <summary>
    /// Use this when you have an absolute coordinate to walk to
    /// </summary>
    /// <param name="destination"></param>
    public abstract void RequestMovement(Vector2 destination);
}