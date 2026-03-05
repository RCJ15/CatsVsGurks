using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ClickableObject), true)]
public class ClickableObjectEditor : Editor
{
    private ClickableObject _target;

    private void OnEnable()
    {
        _target = (ClickableObject)target;   
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

            switch (path)
            {
                case "m_Script":
                    continue;

                case "col":
                    Collider col = _target.GetComponentInChildren<Collider>(true);

                    if (col == null)
                    {
                        col = _target.GetComponentInParent<Collider>(true);
                    }

                    prop.objectReferenceValue = col;
                    continue;

                case "clickables":
                    prop.ClearArray();
                    int i = 0;

                    foreach (Component component in _target.GetComponentsInChildren<Component>(true))
                    {
                        if (component.GetType() == typeof(ClickableObject))
                        {
                            continue;
                        }

                        if (typeof(IClickable).IsAssignableFrom(component.GetType()))
                        {
                            prop.InsertArrayElementAtIndex(i);
                            prop.GetArrayElementAtIndex(i).objectReferenceValue = component;
                            i++;
                        }
                    }
                    continue;
            }

            EditorGUILayout.PropertyField(prop, true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
