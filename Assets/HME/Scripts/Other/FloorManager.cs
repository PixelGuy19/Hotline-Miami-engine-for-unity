using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using Pathfinding;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class FloorManager : MonoBehaviour
{
    public List<GameObject> Floors = new List<GameObject>();
    [SerializeField]
    int FirstLevelFloorIndex = 0, LastLevelFloorIndex = 1;
    bool LevelStarted;
    bool FloorCleared;
    bool LevelCleared;
    private void Reset()
    {
        foreach (Transform Child in gameObject.transform)
        {
            if (Child.childCount > 0 && Child.GetComponent<Tilemap>()) { Floors.Add(Child.gameObject); }
        }
        Pathfinder = FindObjectOfType<AstarPath>();
        SetMain();
    }
#if UNITY_EDITOR
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        SetMain();
    }
    void Update()
    {
        if (Main == null)
        {
            SetMain();
        }
    }
#endif

    private void OnValidate()
    {
        if (FirstLevelFloorIndex < 0) { FirstLevelFloorIndex = 0; }
        if (LastLevelFloorIndex >= Floors.Count) { FirstLevelFloorIndex = Floors.Count - 1; }
        if (FirstLevelFloorIndex >= LastLevelFloorIndex) { FirstLevelFloorIndex = LastLevelFloorIndex; }
    }
    public static FloorManager Main { get; protected set; }
    private void Awake()
    {
        SetMain();
        FloorManager.OnLevelCleared = null;

        foreach(GameObject Floor in Floors)
        {
            Floor?.SetActive(true);
        }
    }
    static void SetMain()
    {
        Main = FindObjectOfType<FloorManager>();
    }

    private void Start()
    {
        StartCoroutine(WaitScan());
        IEnumerator WaitScan()
        {
            yield return null;
            SetFloor(0);
            RecalculatePathfindingArea(Floors.Where((F) => F.activeSelf).First());
            UpdateOnFloorActivatorObjects();
        }
    }

    int CurrentFloorIndex;
    public static int GetFloorIndex()
    {
        if(Main == null) { return 0; }
        return Main.CurrentFloorIndex;
    }
    public static void SetFloor(int Floor)
    {
        if(Main == null) { return; }

        Main.CurrentFloorIndex = Floor;
        if (Main.CurrentFloorIndex < 0) { Main.CurrentFloorIndex = 0; }
        else if (Main.CurrentFloorIndex > Main.Floors.Count - 1) 
        { Main.CurrentFloorIndex = Main.Floors.Count - 1; }

        Main.Floors.RemoveAll((Obj) => Obj == null);
        for (int i = 0; i < Main.Floors.Count; i++)
        {
            Main.Floors[i].SetActive(i == Main.CurrentFloorIndex);
        }
        RecalculatePathfindingArea(Main.Floors[Floor]);
        if (Main.FirstLevelFloorIndex <= Main.CurrentFloorIndex 
            && !Main.LevelStarted) { Main.LevelStarted = true; }
        
       // Floor never < 2
        UpdateOnFloorActivatorObjects();
    }
    public static bool IsLevelStarted()
    {
        if (Main == null) { return false; }
        return Main.LevelStarted;
    }
    public static Action OnLevelCleared;
    public static void ClearFloor()
    {
        if(Main == null) { return; }
        Main.FloorCleared = true;
        if (Main.CurrentFloorIndex == Main.LastLevelFloorIndex)
        {
            Main.LevelCleared = true;
            if(OnLevelCleared != null)
            {
                OnLevelCleared.Invoke();
            }
        }
    }
    public static GameObject GetCurrentFloorObj()
    {
        if(Main == null) { return null; }
        return Main.Floors[GetFloorIndex()];
    }
    public static bool IsFloorCleared()
    {
        if(Main == null) { return true; }
        return Main.FloorCleared;
    }
    public static bool IsLevelCleared()
    {
        if(Main == null) { return false; }
        return Main.LevelCleared;
    }
    public static int GetFirstFloorIndex()
    {
        if (Main == null) { return 1; }
        return Main.FirstLevelFloorIndex;
    }
    public static int GetLastFloorIndex()
    {
        if (Main == null) { return 1; }
        return Main.LastLevelFloorIndex;
    }

    public static float NodeSize { get; private set; }
    [SerializeField]
    protected AstarPath Pathfinder;
    static void RecalculatePathfindingArea(GameObject WallsTilemap)
    {
        if (!Application.isPlaying) { return; }

        Tilemap Map = WallsTilemap.GetComponent<Tilemap>();
        Map.CompressBounds();

        GridGraph Graph = Main.Pathfinder.data.gridGraph;
        Graph.center = Map.cellBounds.center;
        Graph.SetDimensions((int)(Map.cellBounds.size.x / Graph.nodeSize), 
            (int)(Map.cellBounds.size.y / Graph.nodeSize), Graph.nodeSize);
        NodeSize = Graph.nodeSize;

        Main.Pathfinder.Scan();
    }
    static void UpdateOnFloorActivatorObjects()
    {
        foreach (OnFloorActivator Obj in Resources.FindObjectsOfTypeAll<OnFloorActivator>())
        {
            Obj.OnFloorChanged(Main.CurrentFloorIndex);
        }
    }
}