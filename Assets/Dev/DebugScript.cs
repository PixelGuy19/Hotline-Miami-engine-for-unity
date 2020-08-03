using System.Collections;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    [SerializeField]
    GameObject ToShow;
    private void Start()
    {
        StartCoroutine(WaitStart());
        IEnumerator WaitStart()
        {
            yield return new WaitForSeconds(2);
            BlackScreen.ShowScreen(ToShow, 1, 3);
        }
    }
}
