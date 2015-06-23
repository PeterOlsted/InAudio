using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    [CustomPropertyDrawer(typeof(AudioParameters))]
    public class AudioParameterDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            return base.GetPropertyHeight(prop, label) * 4;
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            Rect pos = position;
            pos.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.Slider(pos, prop.FindPropertyRelative("volume"), 0f, 1f, "Volume");
            pos.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.Slider(pos, prop.FindPropertyRelative("pitch"), 0f, 3f, "Pitch");
            pos.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.Slider(pos, prop.FindPropertyRelative("stereoPan"), -1f, 1f, "Stereo Pan");
            pos.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.Slider(pos, prop.FindPropertyRelative("spatialBlend"), 0f, 1f, "Spatial Blend");

            if (EditorGUI.EndChangeCheck())
            {
                prop.serializedObject.ApplyModifiedProperties();
            }
        }
    }

}