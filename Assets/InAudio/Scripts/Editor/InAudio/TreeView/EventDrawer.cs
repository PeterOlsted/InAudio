using InAudioSystem.ExtensionMethods;
using InAudioSystem.InAudioEditor;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.TreeDrawer
{
    public static class EventDrawer
    {
        private static GUIStyle noMargain;
        public static bool EventFoldout(InAudioEventNode node, bool isSelected, out bool clicked)
        {
            clicked = false;
            if (noMargain == null)
            {
                noMargain = new GUIStyle();
                noMargain.margin = new RectOffset(0, 0, 0, 0);
            }

            Rect fullArea = EditorGUILayout.BeginHorizontal();
            Rect area = EditorGUILayout.BeginHorizontal();
            if(isSelected)
                GUI.DrawTexture(area, EditorResources.Instance.GetBackground());
            
            if (node._type != EventNodeType.Event)
                GUILayout.Space(EditorGUI.indentLevel * 16);
            else
                GUILayout.Space(EditorGUI.indentLevel * 24);

            if (node._type != EventNodeType.Event)
            {
                Texture picture;
                if (node.IsFoldedOut || node._children.Count == 0)
                    picture = EditorResources.Instance.Minus;
                else
                    picture = EditorResources.Instance.Plus;

                if (GUILayout.Button(picture, GUIStyle.none, GUILayout.Height(EditorResources.Instance.Minus.height), GUILayout.Width(EditorResources.Instance.Minus.width)))
                {
                    node.IsFoldedOut = !node.IsFoldedOut;
                    Event.current.UseEvent();
                }
          
                TreeNodeDrawerHelper.DrawIcon(GUILayoutUtility.GetLastRect(), EditorResources.Instance.Folder, noMargain);
            }

      
            GUILayout.Space(30);
            EditorGUILayout.LabelField("");
            EditorGUILayout.EndHorizontal();

            Rect labelArea = GUILayoutUtility.GetLastRect();
            
            
            if (node._type != EventNodeType.Event)//As Events are smaller
                labelArea.y += 6;

            if (node._type != EventNodeType.Event)
            {
                labelArea.x += 60;
            }
            else
            {
                labelArea.x += 30;
            }
            EditorGUI.LabelField(labelArea, node.Name);

            EditorGUILayout.EndHorizontal();
            if (Event.current.ClickedWithin(fullArea))
            {
                clicked = true;
            }


            return node.IsFoldedOut;
        }
    }


}
