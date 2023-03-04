using System.Collections;
using UnityEngine;

public abstract class IGameEntity : MonoBehaviour {
    public abstract int Direction { get; }
    public abstract int CameraDirection { get; }
    public abstract int HeadDir { get; }
    public abstract bool IsMonster { get; }
    public abstract GameEntityBaseStatus Status { get; }

    public abstract void ChangeMotion(MotionRequest request);
    public abstract void StopCoroutine(Coroutine coroutine);
    public abstract Coroutine StartCoroutine(IEnumerator coroutine);
}