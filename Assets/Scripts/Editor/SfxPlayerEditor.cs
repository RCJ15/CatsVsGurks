using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SfxPlayer))]
public class SfxPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        if (GUILayout.Button("Cache sounds"))
        {
            SerializedProperty prop = serializedObject.FindProperty("clips");

            prop.ClearArray();
            int i = 0;

            foreach (GUID guid in AssetDatabase.FindAssetGUIDs("a: assets t: AudioClip"))
            {
                AudioClip clip = AssetDatabase.LoadAssetByGUID<AudioClip>(guid);

                if (clip == null)
                {
                    continue;
                }

                prop.InsertArrayElementAtIndex(i);
                prop.GetArrayElementAtIndex(i).objectReferenceValue = clip;
                i++;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
