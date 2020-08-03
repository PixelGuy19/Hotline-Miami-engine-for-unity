using System;
using System.Collections.Generic;
using UnityEngine;

public class Way : MonoBehaviour
{
    public List<Vector3> Waypoints = new List<Vector3>();

    void Reset()
    {
        Waypoints = new List<Vector3>()
        {
            new Vector3(transform.position.x, transform.position.y, 0),
            new Vector3(transform.position.x, transform.position.y + 3, 0),
            new Vector3(transform.position.x + 3, transform.position.y + 3, 0),
            new Vector3(transform.position.x + 3, transform.position.y, 0)
        };
        transform.position = Vector3.zero;
    }
    public void OnValidate()
    {
        transform.position = new Vector3(0.5f, 0.5f);
        MoveToGrid();
    }
    public void MoveToGrid()
    {
        for (int i = 0; i < Waypoints.Count; i++)
        {
            Waypoints[i] = new Vector3(Mathf.RoundToInt(Waypoints[i].x),
                Mathf.RoundToInt(Waypoints[i].y));
        }
    }
}
