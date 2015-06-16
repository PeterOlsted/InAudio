using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    internal static class MusicDrawerHelper
    {
        internal static float GetPropertyHeight(SerializedProperty prop)
        {
            var node = prop.objectReferenceValue as InMusicNode;
            if (node == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight * 2 + 2;
            }
        }

        internal static void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            Rect originalPos = pos;
            float width = pos.width;
            pos.height = EditorGUIUtility.singleLineHeight;

            var node = prop.objectReferenceValue as InMusicNode;

            EditorGUI.PropertyField(pos, prop, label);
            pos.y += 15;
            pos.x += 125;
            pos.width -= 60;



            if (node != null)
            {
                Rect labelPos = originalPos;
                pos.height = EditorGUIUtility.singleLineHeight;
                labelPos.y = labelPos.y + EditorGUIUtility.singleLineHeight;
                labelPos.x += 20;
                EditorGUI.LabelField(labelPos, "Node name:");
                labelPos.x += 100;
                EditorGUI.LabelField(labelPos, node.GetName);

                pos.x = width - 25;
                pos.width = 40;
                if (GUI.Button(pos, "Find"))
                {
                    SearchHelper.SearchFor(node);
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                prop.serializedObject.ApplyModifiedProperties();
            }
        }
    }

    [CustomPropertyDrawer(typeof(InMusicNode))]
    public class InMusicNodeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
            {return MusicDrawerHelper.GetPropertyHeight(prop);}

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
            {MusicDrawerHelper.OnGUI(pos, prop, label);}
    }

    [CustomPropertyDrawer(typeof(InMusicGroup))]
    public class InMusicGroupDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        { return MusicDrawerHelper.GetPropertyHeight(prop); }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        { MusicDrawerHelper.OnGUI(pos, prop, label); }
    }

    [CustomPropertyDrawer(typeof(InMusicFolder))]
    public class InMusicFolderDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        { return MusicDrawerHelper.GetPropertyHeight(prop); }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        { MusicDrawerHelper.OnGUI(pos, prop, label); }
    }

}
