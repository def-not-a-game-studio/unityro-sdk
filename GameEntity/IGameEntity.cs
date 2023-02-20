using System.Collections;
using UnityEngine;

public interface IGameEntity {
    public int Direction { get; }
    public int HeadDir { get; }
    public bool IsMonster { get; }
    public GameEntityBaseStatus Status { get; }

    public void ChangeMotion(MotionRequest request);
    public void StopCoroutine(Coroutine coroutine);
    public Coroutine StartCoroutine(IEnumerator coroutine);
}