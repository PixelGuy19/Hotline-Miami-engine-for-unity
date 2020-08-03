using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class GameObjectTile : TileBase
{
    public GameObject ToSpawn;
    public Sprite Sprite;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = Sprite;
        tileData.gameObject = ToSpawn;
    }
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        if (go != null)
        {
            go.transform.rotation = tilemap.GetTransformMatrix(position).rotation;
        }
        return true;
    }
#if UNITY_EDITOR
    [MenuItem("Assets/Create/HME/GameObject Tile")]
    public static void CreateGameObjectTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save GameObject Tile", 
            "New GameObject Tile", "Asset", "Save GameObject Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GameObjectTile>(), path);
    }
#endif
}