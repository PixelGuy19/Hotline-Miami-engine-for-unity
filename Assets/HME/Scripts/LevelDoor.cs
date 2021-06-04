using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    [SerializeField]
    Collider2D Door = default;
    private void OnTriggerExit2D(Collider2D collision)
    {
        Door.enabled = true;
        FloorManager.OnLevelCleared += () => { Door.enabled = false; };
    }
}
