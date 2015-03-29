using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    [CustomPropertyDrawer(typeof(InAudioSystem.Runtime.MecanimParameterList))]
    public class MecanimParameterListDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return base.GetPropertyHeight(prop, label);
        }


        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            prop.serializedObject.Update();
            prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, "Volume Control");

            if (prop.isExpanded)
            {
                InAudioSystem.ReorderableList.ReorderableListGUI.ListField(prop.FindPropertyRelative("ParameterList"));
            }
            prop.serializedObject.ApplyModifiedProperties();
        }
    }
} 