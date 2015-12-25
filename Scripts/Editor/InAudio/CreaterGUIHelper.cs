using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public static class CreaterGUIHelper
    {
        public static GUISkin GetEditorSkin()
        {
            GUISkin builtinSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

            EditorUtility.CopySerialized(builtinSkin, GUI.skin);
            UpdateStyles(GUI.skin);
            return GUI.skin;
        }

        private static void UpdateStyles(GUISkin skin)
        {
            if (SearchFieldStyle == null)
            {

                SearchFieldStyle = skin.FindStyle("ToolbarSeachTextField");
                SearchCancelStyle = skin.FindStyle("ToolbarSeachCancelButton");
                ToolbarStyle = skin.FindStyle("Toolbar");
            }
        }

        public static GUIStyle SearchFieldStyle { get; private set; }

        public static GUIStyle SearchCancelStyle { get; private set; }

        public static GUIStyle ToolbarStyle { get; private set; }
    }

}