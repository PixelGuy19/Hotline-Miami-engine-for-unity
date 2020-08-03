using UnityEngine;

public class LinkedDestroy : MonoBehaviour
{
    [SerializeField]
    GameObject Linked;
    private void OnDestroy()
    {
        Destroy(Linked);
    }
}
