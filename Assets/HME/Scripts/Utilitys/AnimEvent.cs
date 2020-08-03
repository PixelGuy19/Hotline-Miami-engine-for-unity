using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    [SerializeField]
    UnityEvent OnAnimationEvent;
    public void InvokeEvent()
    {
        OnAnimationEvent.Invoke();
    }
    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}
