using InAudioSystem.ReorderableList;
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace InAudioSystem.InAudioEditor
{
    //[CanEditMultipleObjects]// Does not work currently, work in progress
    [CustomPropertyDrawer(typeof(InEventHookMusicControl))]
    public class InEventHookMusicControlDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var propPos = position;
            propPos.width -= 65;
            var controlPos = position;
            controlPos.width = 60;
            controlPos.x = position.width - controlPos.width + 30;
            EditorGUI.PropertyField(propPos, property.FindPropertyRelative("MusicGroup"));
            //EditorGUI.PropertyField(controlPos, property.FindPropertyRelative("PlaybackControl"));
            
            property.FindPropertyRelative("PlaybackControl").enumValueIndex = (int)(MusicState)EditorGUI.EnumPopup(controlPos, (MusicState)property.FindPropertyRelative("PlaybackControl").intValue);
            var buttonPos = controlPos;
            buttonPos.width = 10;
            controlPos.x += 65;
            
            //ReorderableListGUI.ListField(property);
            EditorGUI.EndProperty();
        }
    }
}
