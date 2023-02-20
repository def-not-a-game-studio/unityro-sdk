using System.Collections;
using UnityEngine;

public partial class CoreGameEntity : MonoBehaviour, IGameEntity {
    public int Direction { get; }
    public int HeadDir { get; }
    public bool IsMonster { get; }
    public GameEntityBaseStatus Status { get; }
    
    public void ChangeMotion(MotionRequest request) {
        throw new System.NotImplementedException();
    }

    public void StopCoroutine(Coroutine coroutine) {
        throw new System.NotImplementedException();
    }

    public Coroutine StartCoroutine(IEnumerator coroutine) {
        throw new System.NotImplementedException();
    }
}