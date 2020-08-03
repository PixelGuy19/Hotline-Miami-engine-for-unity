using System.Linq;
using UnityEngine;

public class OnFloorActivator : MonoBehaviour
{
    public int[] ActiveOnFloors = new int[] { 0, 1 };
    public void OnFloorChanged(int Floor)
    {
        ActiveOnFloors = ActiveOnFloors.Distinct().ToArray();
        foreach (int Index in ActiveOnFloors)
        {
            if(Index == Floor)
            {
                gameObject.SetActive(true);
                return;
            }
        }
        gameObject.SetActive(false);
    }
}
