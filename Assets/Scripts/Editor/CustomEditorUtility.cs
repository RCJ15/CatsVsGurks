using UnityEngine;

public static class CustomEditorUtility
{
    public static GUIStyle ItalicLabelStyle
    {
        get
        {
            if (_italicLabelStyle == null)
            {
                _italicLabelStyle = new GUIStyle(GUI.skin.label);

                _italicLabelStyle.fontStyle = FontStyle.Italic;
            }

            return _italicLabelStyle;
        }
    }

    private static GUIStyle _italicLabelStyle;

    public static GUIStyle ItalicFadedLabelStyle
    {
        get
        {
            if (_italicFadedLabelStyle == null)
            {
                _italicFadedLabelStyle = new GUIStyle(ItalicLabelStyle);

                _italicFadedLabelStyle.normal.textColor /= 1.5f;
                _italicFadedLabelStyle.onNormal.textColor /= 1.5f;
            }

            return _italicFadedLabelStyle;
        }
    }

    private static GUIStyle _italicFadedLabelStyle;
}
