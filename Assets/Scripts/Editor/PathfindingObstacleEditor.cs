using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathfindingObstacle))]
public class PathfindingObstacleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        SerializedProperty typeProp = FindProp("type");

        DrawProp(typeProp);

        serializedObject.ApplyModifiedProperties();

        PathfindingObstacle.Type type = (PathfindingObstacle.Type)typeProp.enumValueIndex;

        switch (type)
        {
            case PathfindingObstacle.Type.Box:
                DrawProp("size");
                break;

            case PathfindingObstacle.Type.Circle:
                DrawProp("radius");
                break;
        }

        EditorGUILayout.Space();

        DrawProp("block");
        DrawProp("cost");

        serializedObject.ApplyModifiedProperties();
    }

    private SerializedProperty FindProp(string name) => serializedObject.FindProperty(name);
    private void DrawProp(SerializedProperty prop) => EditorGUILayout.PropertyField(prop);
    private void DrawProp(string name) => EditorGUILayout.PropertyField(FindProp(name));
}
