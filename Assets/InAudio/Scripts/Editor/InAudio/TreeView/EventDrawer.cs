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
                GUI.DrawTexture(area, EditorResources.Background);
            
            if (node._type != EventNodeType.Event)
                GUILayout.Space(EditorGUI.indentLevel * 16);
            else
                GUILayout.Space(EditorGUI.indentLevel * 24);

            if (node._type != EventNodeType.Event)
            {
                Texture picture;
                if (node.IsFoldedOut || node._children.Count == 0)
                    picture = EditorResources.Minus;
                else
                    picture = EditorResources.Plus;

                GUILayout.Label(picture, noMargain, GUILayout.Height(EditorResources.Minus.height), GUILayout.Width(EditorResources.Minus.width));
                
                Rect foldRect = GUILayoutUtility.GetLastRect();
                if (Event.current.ClickedWithin(foldRect))
                {
                    Event.current.Use();
                }
                if (Event.current.MouseUpWithin(foldRect))
                {
                    node.IsFoldedOut = !node.IsFoldedOut;
                    Event.current.Use();
                }
          
                TreeNodeDrawerHelper.DrawIcon(GUILayoutUtility.GetLastRect(), EditorResources.Folder, noMargain);
            }

      
            GUILayout.Space(30);
            EditorGUILayout.LabelField("");
            EditorGUILayout.EndHorizontal();

            Rect labelArea = GUILayoutUtility.GetLastRect();
            Rect buttonArea = GUILayoutUtility.GetLastRect();
            if (!node.IsRoot)
            {
                buttonArea.x = buttonArea.x + 56 + EditorGUI.indentLevel*16;
                buttonArea.width = 20;
                buttonArea.height = 14;
                if (node._type != EventNodeType.Event)
                {
                    GUI.Label(buttonArea, EditorResources.Up, noMargain);
                    if (Event.current.ClickedWithin(buttonArea))
                    {
                        NodeWorker.MoveNodeOneUp(node);
                        Event.current.Use();
                    }
                    buttonArea.y += 15;
                    GUI.Label(buttonArea, EditorResources.Down, noMargain);
                    if (Event.current.ClickedWithin(buttonArea))
                    {
                        NodeWorker.MoveNodeOneDown(node);
                        Event.current.Use();
                    }
                }
                else
                {
                    buttonArea.x -= 10;
                    GUI.Label(buttonArea, EditorResources.Up, noMargain);
                    if (Event.current.ClickedWithin(buttonArea))
                    {
                        NodeWorker.MoveNodeOneUp(node);
                        Event.current.Use();
                    }
                    buttonArea.x += 15;
                    GUI.Label(buttonArea, EditorResources.Down, noMargain);
                    if (Event.current.ClickedWithin(buttonArea))
                    {
                        NodeWorker.MoveNodeOneDown(node);
                        Event.current.Use();
                    }
                }
                labelArea.x += 25;

            }
            if (node._type != EventNodeType.Event)//As Events are smaller
                labelArea.y += 6;
            labelArea.x += 65;
            EditorGUI.LabelField(labelArea, node.Name);

            EditorGUILayout.EndHorizontal();
            if (Event.current.ClickedWithin(fullArea, 0))
            {
                clicked = true;
            }
            if (Event.current.type == EventType.MouseDown /*&& lastClickNode == node */&& Event.current.button == 0 && fullArea.Contains(Event.current.mousePosition) && DragAndDrop.objectReferences.Length == 0)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = new UnityEngine.Object[] { node };
                DragAndDrop.StartDrag("Event Node Drag");
                Event.current.Use();
            }

            return node.IsFoldedOut;
        }
    }


}
