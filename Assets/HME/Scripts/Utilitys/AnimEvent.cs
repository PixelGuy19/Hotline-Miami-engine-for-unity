using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    [SerializeField]
    UnityEvent OnAnimationEvent = default;
    public void InvokeEvent()
    {
        OnAnimationEvent.Invoke();
    }
    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}
