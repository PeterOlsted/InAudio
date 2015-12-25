using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    [CustomPropertyDrawer(typeof(InAudioNode))]
    public class InAudioNodePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
        {
            var node = prop.objectReferenceValue as InAudioNode;
            if (node == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight * 2;
            } 
        }

        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginProperty(pos, label, prop);

            Rect originalPos = pos;
            float width = pos.width;
            pos.height = EditorGUIUtility.singleLineHeight;
            var node = prop.objectReferenceValue as InAudioNode;
           
                 
            EditorGUI.PropertyField(pos, prop, label);
            pos.y += 15;
            pos.x += 125;
            pos.width -= 60;
            
            

            if (node != null)
            {
                
                Rect labelPos = originalPos;
                pos.height = EditorGUIUtility.singleLineHeight;
                labelPos.y = labelPos.y + EditorGUIUtility.singleLineHeight;
                labelPos.x += 14;
                EditorGUI.LabelField(labelPos, "Node name:");
                labelPos.x += 106;
                EditorGUI.LabelField(labelPos, node.Name);


                pos.x = width - 25;
                pos.width = 40;
                if (GUI.Button(pos, "Find"))
                {
                    SearchHelper.SearchFor(node);
                }
            }
            EditorGUI.EndProperty();
        }
    }

}
