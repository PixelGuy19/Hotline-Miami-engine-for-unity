using UnityEngine;

public class VehicleTrigger : MonoBehaviour
{
    [SerializeField]
    Animator MyAnim;
    public static bool Closed;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Closed || FloorManager.IsLevelCleared())
        {
            MyAnim.SetBool("IsOpened", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        MyAnim.SetBool("IsOpened", false);
    }
}
