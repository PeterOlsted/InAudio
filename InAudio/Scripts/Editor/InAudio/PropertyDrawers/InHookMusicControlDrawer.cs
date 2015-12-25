using InAudioSystem.ReorderableList;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    //[CanEditMultipleObjects]// Does not work currently, work in progress
    [CustomPropertyDrawer(typeof(InHookMusicControl))]
    public class InHookMusicControlDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label);
            if (property.isExpanded)
            {
                ReorderableListGUI.ListField(property.FindPropertyRelative("MusicControls"), EditorGUIUtility.singleLineHeight * 2);
            }
            EditorGUI.EndProperty();
        }
    }
}
