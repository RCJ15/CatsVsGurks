using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Unit), true)]
public class UnitEditor : Editor
{
    private Unit _target;

    private void OnEnable()
    {
        _target = (Unit)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        SerializedProperty prop = serializedObject.GetIterator();
        bool enterChildren = true;

        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;

            string path = prop.propertyPath;
            if (path == "m_Script")
            {
                continue;
            }

            if (path == "components")
            {
                prop.ClearArray();

                foreach (UnitComponent component in _target.GetComponentsInChildren<UnitComponent>())
                {
                    int i = prop.arraySize;
                    prop.InsertArrayElementAtIndex(i);

                    prop.GetArrayElementAtIndex(i).objectReferenceValue = component;
                }

                continue;
            }

            EditorGUILayout.PropertyField(prop, true);

            // Display DPS
            if (path == "attackDelay")
            {
                float damage = serializedObject.FindProperty("damage").floatValue;
                float attackDelay = serializedObject.FindProperty("attackDelay").floatValue;

                EditorGUILayout.LabelField("DPS: " + damage / attackDelay, CustomEditorUtility.ItalicFadedLabelStyle);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
