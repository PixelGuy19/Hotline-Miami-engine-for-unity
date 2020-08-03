using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelEditorWindow : EditorWindow
{
    [MenuItem("Window/HME/Level editor")]
    public static void Init()
    {
        LevelEditorWindow Window = GetWindow<LevelEditorWindow>("Level editor", true);
        Window.minSize = new Vector2(400, 150);
    }
    GameObject LastSelectedFloor;
    private void OnSelectionChange()
    {
        if (FloorManager.Main != null)
        {
            if (FloorManager.Main.Floors.Contains(Selection.activeGameObject))
            {
                for (int i = 0; i < FloorManager.Main.Floors.Count; i++)
                {
                    GameObject Floor = FloorManager.Main.Floors[i];
                    if (Floor == Selection.activeGameObject)
                    {
                        FloorManager.SetFloor(i);
                        LastSelectedFloor = FloorManager.GetCurrentFloorObj();
                    }
                }
            }
        }
        Repaint();
    }

    public int[] SelectedLevels = new int[] { 0 };
    SerializedObject Me;
    SerializedProperty Levels;
    private void OnEnable()
    {
        Me = new SerializedObject(this);
        Tilemap.tilemapTileChanged += OnTilemapChanged;

        void OnTilemapChanged(Tilemap arg1, Tilemap.SyncTile[] arg2)
        {
            GameObject TilemapObj = GameObject.Find(arg1.name);
            if (GameObject.Find(arg1.name) == null || TilemapObj.transform.childCount == 0) { return; }
            foreach (Tilemap.SyncTile STile in arg2)
            {
                if (STile.tile == null || STile.tile.GetType() != typeof(Tile)) { continue; }
                if ((STile.tile as Tile).colliderType == Tile.ColliderType.None)
                {
                    arg1.SetTile(STile.position, null);
                    Debug.LogWarning("Set floor tiles to floor object");
                }
            }
        }
    }


    GameObjectCreation CreationMethod;
    private void OnGUI()
    {
        bool HasMain = FloorManager.Main != null;
        GUI.enabled = HasMain;
        CreationMethod = (GameObjectCreation)EditorGUILayout.EnumPopup("Creation method", CreationMethod);
        DropAreaGUI("Add object");
        if (GUILayout.Button("Add way"))
        {
            AddWay();
        }
        GUI.enabled = CreationMethod == GameObjectCreation.OutsideFloorsWithOnFloorActivator;
        Levels = Me.FindProperty("SelectedLevels");
        EditorGUILayout.PropertyField(Levels, true);
        Me.ApplyModifiedProperties();

        GUI.enabled = CreationMethod == GameObjectCreation.OutsideFloors;
        GUILayout.Space(10);
        if (GUILayout.Button("Add floor"))
        {
            AddFloor();
        }
        GUI.enabled = !HasMain;

        if (GUILayout.Button("Setup builder"))
        {
            CreateGrid();
        }
    }
    GameObject CreateObj(string Name)
    {
        GameObject Out;
        Out = new GameObject(Name);
        SetParent(ref Out);

        return Out;
    }
    GameObject InstantiateObj(ref GameObject Obj)
    {
        GameObject Out = Instantiate(Obj);
        Out.name = Out.name.Replace("(Clone)", "");
        SetParent(ref Out);
        Out.transform.position =
            new Vector3(Out.transform.position.x, Out.transform.position.y, 0);
        return Out;
    }

    void AddFloor()
    {
        FloorManager.Main.Floors.RemoveAll(item => item == null);
        int FloorIndex = FloorManager.Main.Floors.Count + 1;
        GameObject Root = Instantiate(Resources.Load<GameObject>("FloorPreset"),
           CreationMethod == GameObjectCreation.OutsideFloors ? FloorManager.Main.transform
           : LastSelectedFloor?.transform);
        Root.name = "Floor-" + FloorIndex;
        FloorManager.Main.Floors.Add(Root);
        Selection.activeGameObject?.SetActive(false);
        Selection.activeGameObject = Root;
    }
    void CreateGrid()
    {
        GameObject GridObj = CreateObj("Grid");
        Instantiate(Resources.Load<GameObject>("Pathfinder"), GridObj.transform);
        Grid Grid = GridObj.AddComponent<Grid>();
        Grid.cellSize = new Vector3(1, 1, 0);
        GridObj.AddComponent<FloorManager>();
        AddFloor();
    }
    void AddWay()
    {
        GameObject WayObj = CreateObj("Path");
        WayObj.transform.position = GetScreenCenter();
        WayObj.AddComponent<Way>();
        Selection.activeObject = WayObj;
    }
    Vector3 GetScreenCenter()
    {
        return SceneView.lastActiveSceneView.camera.transform.position;
    }
    void SetParent(ref GameObject Obj)
    {
        Transform Tr = Obj.transform;
        if (CreationMethod == GameObjectCreation.OnSelectedFloor)
        {
            if (LastSelectedFloor == null) { return; }
            Tr.parent = LastSelectedFloor.transform;
        }
        if (CreationMethod == GameObjectCreation.OutsideFloors)
        {
            Tr.parent = FloorManager.Main.transform;
        }
        if (CreationMethod == GameObjectCreation.OutsideFloorsWithOnFloorActivator)
        {
            Tr.parent = FloorManager.Main.transform;
            OnFloorActivator Activator = Obj.gameObject.AddComponent<OnFloorActivator>();
            Activator.ActiveOnFloors = SelectedLevels;
        }
    }
    void ResetFloor()
    {
        FloorManager.SetFloor(FloorManager.GetFloorIndex());
    }

    //https://gist.github.com/bzgeb/3800350
    void DropAreaGUI(string Text)
    {
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, EditorGUIUtility.singleLineHeight * 2, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, Text);

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object dragged_object in DragAndDrop.objectReferences)
                    {
                        if (dragged_object.GetType() == typeof(GameObject))
                        {
                            GameObject Obj = dragged_object as GameObject;
                            Obj.transform.position = GetScreenCenter();
                            Selection.activeObject = InstantiateObj(ref Obj);
                            ResetFloor();
                        }
                    }
                }
                break;
        }
    }
    enum GameObjectCreation { OnSelectedFloor, OutsideFloors, OutsideFloorsWithOnFloorActivator }
}