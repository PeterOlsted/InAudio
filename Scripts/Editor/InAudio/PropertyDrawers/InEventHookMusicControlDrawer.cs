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
            return base.GetPropertyHeight(property, label) * 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var propPos = position;
            propPos.width -= 65;
            var controlPos = position;
            controlPos.width = 60;
            controlPos.x = position.width - controlPos.width + 30;
            
            property.FindPropertyRelative("PlaybackControl").enumValueIndex = (int)(MusicState)EditorGUI.EnumPopup(controlPos, (MusicState)property.FindPropertyRelative("PlaybackControl").intValue);
            propPos.width -= 20;
            propPos.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.ObjectField(propPos, property.FindPropertyRelative("MusicGroup"), typeof(InMusicGroup), GUIContent.none);
            var musicGroup = property.FindPropertyRelative("MusicGroup").objectReferenceValue as InMusicGroup;
            if (musicGroup != null)
            {
                Rect labelArea = position;
                labelArea.y += EditorGUIUtility.singleLineHeight;
                labelArea.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(labelArea, musicGroup.GetName);

                Rect findArea = labelArea;
                findArea.x = position.width - 105;
                findArea.width = 40;
                if (GUI.Button(findArea, "Find"))
                {
                    SearchHelper.SearchFor(musicGroup);
                }
            }

            var buttonPos = controlPos;
            buttonPos.width = 10;
            controlPos.x += 65;
            
           
            
            EditorGUI.EndProperty();
            
        }
    }
}
