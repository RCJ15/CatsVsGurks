using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitInfo))]
public class UnitInfoEditor : Editor
{
    private const string AI_PROPERTY_PATH = "ai";
    private static readonly List<(UnitDefinitionAttribute, Type)> _units = new();

    [InitializeOnLoadMethod]
    private static void FindUnits()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                UnitDefinitionAttribute attribute = type.GetCustomAttribute<UnitDefinitionAttribute>(false);

                if (attribute == null)
                {
                    continue;
                }

                _units.Add((attribute, type));
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        SerializedProperty iterator = serializedObject.GetIterator();

        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            if ("m_Script" == iterator.propertyPath)
            {
                continue;
            }

            if (iterator.propertyPath == AI_PROPERTY_PATH)
            {
                EditorGUILayout.Space();

                Rect rect = EditorGUILayout.GetControlRect();

                rect = EditorGUI.PrefixLabel(rect, new GUIContent("AI"));

                SerializedProperty aiProp = iterator;

                Type selectedType = Type.GetType(aiProp.stringValue);
                GUIContent content = new GUIContent(selectedType == null ? "<none>" : selectedType.Name);

                if (!EditorGUI.DropdownButton(rect, content, FocusType.Keyboard))
                {
                    continue;
                }

                GenericMenu menu = new();

                foreach (var pair in _units)
                {
                    Team team = pair.Item1.Team;
                    Type type = pair.Item2;
                    menu.AddItem(new GUIContent(team.ToString() + "/" + type.FullName), selectedType == type, () =>
                    {
                        using (SerializedObject obj = new SerializedObject(target))
                        {
                            obj.FindProperty(AI_PROPERTY_PATH).stringValue = type.AssemblyQualifiedName;
                            obj.ApplyModifiedProperties();
                        }
                    });
                }

                menu.ShowAsContext();
                
                continue;
            }

            EditorGUILayout.PropertyField(iterator, true);

            enterChildren = false;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
