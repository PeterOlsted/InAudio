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
            prop.serializedObject.Update();

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
                if (!node.IsRootOrFolder)
                {
                    pos.x = 160;
                    EditorGUI.LabelField(pos, node.Name);
                    pos.x -= 80;
                    EditorGUI.LabelField(pos, "Node name:");
                }
                else
                {
                    Rect warningArea = originalPos;
                    warningArea.height = EditorGUIUtility.singleLineHeight;
                    warningArea.width -= 40;
                    warningArea.y = pos.y;
                    EditorGUI.HelpBox(warningArea, "Cannot play Folder node", MessageType.Error);
                }

                pos.x = width - 25;
                pos.width = 40;
                if (GUI.Button(pos, "Find"))
                {
                    SearchHelper.SearchFor(node);
                }
            }
            prop.serializedObject.ApplyModifiedProperties();
        }
    }

}
