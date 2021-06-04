using UnityEngine;

public class StairsTrigger : MonoBehaviour
{
    [SerializeField]
    int ToFloor = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (FloorManager.IsLevelStarted() && FloorManager.IsFloorCleared()
            || !FloorManager.IsLevelStarted())
        {
            BlackScreen.FadeInOut(0.2f, () =>
            {
                FloorManager.SetFloor(ToFloor);
            });
        }
    }
}