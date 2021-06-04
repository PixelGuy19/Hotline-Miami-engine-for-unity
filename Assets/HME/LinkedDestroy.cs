using UnityEngine;

public class LinkedDestroy : MonoBehaviour
{
    [SerializeField]
    GameObject Linked = default;
    private void OnDestroy()
    {
        Destroy(Linked);
    }
}
