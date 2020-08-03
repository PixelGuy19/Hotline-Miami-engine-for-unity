using System.Linq;
using System.Collections;
using UnityEngine;

public class ElevatorFloorTrigger : MonoBehaviour
{
    [SerializeField]
    SpriteAnim MyAnim;
    [Tooltip("If don't set elevator will go to the next floor(+1) and when all floors cleared will go to the first floor of the level.")]
    [SerializeField]
    int ToFloor;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ToFloor != FloorManager.GetFloorIndex() || FloorManager.IsLevelStarted())
        {
            BlackScreen.FadeInOut(MyAnim.FrameTime * MyAnim.Sprites.Length);
            PlayerBase.LockControls(true);
            MyAnim.Play();
        }
    }

    bool IsElevatorClosed = false;
    public void ElevatorClosedOrOpened()
    {
        Debug.Log("Elevator working");
        if (!IsElevatorClosed)
        {
            if (FloorManager.IsLevelStarted())
            {
                //Проработать механику, подробнее в Tooltip
                int FloorToSet = FloorManager.GetFloorIndex() + 1;
                if (FloorToSet > FloorManager.GetLastFloorIndex())
                { FloorToSet = FloorManager.GetFirstFloorIndex(); enabled = false; }
                FloorManager.SetFloor(FloorToSet);
            }
            else
            {
                FloorManager.SetFloor(ToFloor);
            }
            MyAnim.Sprites = MyAnim.Sprites.Reverse().ToArray();
            IsElevatorClosed = true;
            Debug.Log("Elevator closed");
            StartCoroutine(SlowPlayer());
        }
        else
        {
            MyAnim.Sprites = MyAnim.Sprites.Reverse().ToArray();
            PlayerBase.LockControls(false);
            IsElevatorClosed = false;
            Debug.Log("Elevator opened");
        }

        IEnumerator SlowPlayer()
        {
            yield return new WaitForSeconds(0.5f);
            MyAnim.Play();
        }
    }
}
