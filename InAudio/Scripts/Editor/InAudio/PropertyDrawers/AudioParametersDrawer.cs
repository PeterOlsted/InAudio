using UnityEngine;
using UnityEditor;

namespace InAudioSystem.InAudioEditor
{
    [CustomPropertyDrawer(typeof (AudioParameters))]
    public class AudioParametersDrawer : PropertyDrawer
    {
        private static GUIContent blendLabel;
        private static GUIContent stereoLabel;

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            if (!prop.isExpanded)
            {
                return base.GetPropertyHeight(prop, label);
            }
            else
            {
                return base.GetPropertyHeight(prop, label)*8 + 5;
            }
        }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, prop);

            if (blendLabel == null)
            {
                blendLabel = new GUIContent("Spatial Blend (2D-3D)");
            }
            if (stereoLabel == null)
            {
                stereoLabel = new GUIContent("Stereo Pan (left-right)");
            }

            pos.height = EditorGUIUtility.singleLineHeight;
            prop.isExpanded = EditorGUI.Foldout(pos, prop.isExpanded, label);
            if (prop.isExpanded)
            {
                pos.y += EditorGUIUtility.singleLineHeight;
                pos.x += 14;
                pos.width -= 14;

                var volume = prop.FindPropertyRelative("volume");
                var pitch = prop.FindPropertyRelative("pitch");
                var stereoPan = prop.FindPropertyRelative("stereoPan");
                var spatialBlend = prop.FindPropertyRelative("spatialBlend");
                var audioMixer = prop.FindPropertyRelative("audioMixer");
                var setMixer = prop.FindPropertyRelative("setMixer");

                EditorGUI.PropertyField(pos, volume);
                pos.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(pos, pitch);
                pos.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(pos, stereoPan, stereoLabel);
                pos.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(pos, spatialBlend, blendLabel);
                pos.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(pos, setMixer);

                GUI.enabled = setMixer.boolValue;
                pos.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(pos, audioMixer);
                GUI.enabled = true;


                pos.y += EditorGUIUtility.singleLineHeight;
                pos.x += pos.width - 75;
                pos.width = 75;
                if (GUI.Button(pos, "Reset All"))
                {
                    volume.floatValue = 1.0f;
                    pitch.floatValue = 1.0f;
                    stereoPan.floatValue = 0.0f;
                    spatialBlend.floatValue = 1.0f;
                    audioMixer.objectReferenceValue = null;
                    setMixer.boolValue = false;
                }
            }
            EditorGUI.EndProperty();
        }
    }
}