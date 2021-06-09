using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorutineDisabling : MonoBehaviour
{
    IEnumerator HelloFuckingWorld()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("A platapus FUCKER!");
    }
    private void Start()
    {
        StartCoroutine(HelloFuckingWorld());
        enabled = false;        
    }
}
