using System.Collections;
using UnityEngine;

public abstract class CoreGameEntity : MonoBehaviour {
    
    public GameEntityBaseStatus Status { get; }

    public abstract void ChangeMotion(MotionRequest request);
}