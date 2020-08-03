using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerEvent : MonoBehaviour
{
    public TriggerEvent EventType;
    public UnityEvent OnEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (EventType == TriggerEvent.OnTriggerEnter) { StartCoroutine(EventStarter()); }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (EventType == TriggerEvent.OnTriggerExit) { StartCoroutine(EventStarter()); }
    }
    public float EventTimeOffset;
    IEnumerator EventStarter()
    {
        yield return new WaitForSeconds(EventTimeOffset);
        OnEvent.Invoke();
    }

    public enum TriggerEvent { OnTriggerEnter, OnTriggerExit }
}