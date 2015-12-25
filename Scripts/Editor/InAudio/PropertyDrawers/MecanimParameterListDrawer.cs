using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    [CustomPropertyDrawer(typeof(Runtime.MecanimParameterList))]
    public class MecanimParameterListDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return base.GetPropertyHeight(prop, label);
        }


        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, prop);
            prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, "Volume Control");

            if (prop.isExpanded)
            {
                ReorderableList.ReorderableListGUI.ListField(prop.FindPropertyRelative("ParameterList"));
            }
            EditorGUI.EndProperty();
        }
    }
} 