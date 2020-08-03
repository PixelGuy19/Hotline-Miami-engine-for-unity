using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorotineTry : MonoBehaviour
{
    IEnumerator Cor1()
    {
        Coroutine A = StartCoroutine(Cor2());
        yield return new WaitForSeconds(1);
        Debug.Log("B");
        StopCoroutine(A);
    }
    IEnumerator Cor2()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("A");
    }
    private void Start()
    {
        StartCoroutine(Cor1());
    }
}
