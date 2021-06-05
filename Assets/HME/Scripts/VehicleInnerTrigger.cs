using System.Collections;
using System.Linq;
using UnityEngine;

public class VehicleInnerTrigger : MonoBehaviour
{
    [SerializeField]
    Animator MyAnim = default;
    [SerializeField]
    Transform ExitPos = default;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(DoorCloser());
            IEnumerator DoorCloser()
            {
                if (ExitPos == null) { yield break; }

                PlayerBase.LockControls(true);
                MyAnim.SetBool("IsOpened", false);
                yield return new WaitForSeconds(1f);

                GameObject StartScreen = Resources.FindObjectsOfTypeAll<GameObject>()
                    .Where((GameObject Obj) => Obj.name == "LevelStartScreen").First(); //Replace this shit
                BlackScreen.ShowScreen(StartScreen, 1, 3, () =>
                   {
                       if (FloorManager.IsLevelCleared())
                       {
                           FloorManager.SetFloor(FloorManager.GetFloorIndex() + 1);
                       }
                       else
                       {
                           FloorManager.SetFloor(FloorManager.GetFirstFloorIndex());
                       }

                       collision.transform.position = ExitPos.position;
                       VehicleTrigger.Closed = true;
                   },
                  () =>
                  {
                      PlayerBase.LockControls(false);
                  });
            }
        }
    }
}