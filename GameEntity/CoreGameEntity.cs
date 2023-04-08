using UnityEngine;

public abstract class CoreGameEntity : MonoBehaviour {
    
    public abstract GameEntityBaseStatus Status { get; }

    public abstract void ChangeMotion(MotionRequest request);

    public abstract void Init(GameEntityBaseStatus gameEntityBaseStatus);
}