using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(GameObjectTile))]
public class GameObjectTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();       
        //GameObjectTile Obj = target as GameObjectTile;
        //Obj.ToSpawn = (GameObject)EditorGUILayout.ObjectField("To Spawn", Obj.ToSpawn, typeof(GameObject), true);
        //Obj.Sprite = (Sprite)EditorGUILayout.ObjectField("Editor sprite", Obj.Sprite, typeof(Sprite), true);
        SerializedObject Obj = new SerializedObject(target);
        EditorGUILayout.PropertyField(Obj.FindProperty("ToSpawn"));
        EditorGUILayout.PropertyField(Obj.FindProperty("Sprite"));
        Obj.ApplyModifiedProperties();
    }
}
