using System;
using System.Collections.Generic;
using System.Linq;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{

    public class TreeDrawer<T> where T : Object, InITreeNode<T>
    {

        public TreeDrawer(EditorWindow window)
        {
            Window = window;
            EditorApplication.update += Update;
        }

        public T SelectedNode
        {
            get { return selectedNode; }
            set { selectedNode = value; }
        }


        public Vector2 ScrollPosition;

        public delegate void OnContextDelegate(T node);
        public OnContextDelegate OnContext;

        public delegate bool OnNodeDrawDelegate(T node, bool isSelected, out bool clicked);
        public OnNodeDrawDelegate OnNodeDraw;

        public delegate void OnDropDelegate(T node, Object[] objects);
        public OnDropDelegate OnDrop;

        public delegate bool CanDropObjectsDelegate(T node, Object[] objects);
        public CanDropObjectsDelegate CanDropObjects;

        public delegate bool CanPlaceHereDelegate(T newParent, T toPlace);
        public CanPlaceHereDelegate CanPlaceHere = (P, N) => true;

        public delegate void AssignNewParentDelegate(T newParent, T node, int index = -1);

        public AssignNewParentDelegate AssignNewParent = (p, n, i) =>
        {
            n._getParent = p;
            if (i == -1)
            {
                p._getChildren.Insert(0, n);
            }
            else
            {
                p._getChildren.Insert(i, n);
            }
        };

        public delegate void DeattachFromParentDelegate(T node);
        public DeattachFromParentDelegate DeattachFromParent = node => node._getParent._getChildren.Remove(node);

        public bool DrawTree(T treeRoot, Rect area)
        {
            dirty = false;
            if (SelectedNode == null)
                selectedNode = treeRoot;
            if (SelectedNode == null)
                return false;

            KeyboardControl();

            int startIndent = EditorGUI.indentLevel;
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, false, true);

            if (treeRoot == null || OnNodeDraw == null)
                return true;


            if (selectedNode.IsFiltered)
                selectedNode = treeRoot;

            if (triggerFilter)
            {
                FilterNodes(treeRoot, filterFunc);

                triggerFilter = false;
            }

            toDrawArea.Clear();

            DrawTree(treeRoot, EditorGUI.indentLevel);

            if (DragAndDrop.objectReferences.FirstOrDefault() as T != null && EditorWindow.focusedWindow == Window)
            {
                dirty = true;
            }

            if (EditorWindow.focusedWindow == Window)
            {
                foreach (var drawArea in toDrawArea)
                {
                    var fullArea = drawArea.Big;
                    var smallArea = drawArea.Small;
                    if (DragAndDrop.objectReferences.FirstOrDefault() as T != null)
                    {
                        if (smallArea.Contains(Event.current.mousePosition))
                        {
                            GUIDrawRect(smallArea, EditorResources.Instance.GetBackground().GetPixel(0, 0) * 0.7f);
                            break;
                        }
                        else if (fullArea.Contains(Event.current.mousePosition))
                        {
                            DrawAround(area, smallArea, fullArea);
                            break;
                        }
                    }
                    else if (CanDropObjects(drawArea.Node, DragAndDrop.objectReferences) && fullArea.Contains(Event.current.mousePosition))
                    {

                        DrawAround(area, smallArea, fullArea);
                        break;

                    }
                }

            }


            if (Event.current.type == EventType.DragExited)
            {
                dirty = true;
                Window.Repaint();
            }

            EditorGUILayout.EndScrollView();

            EditorGUI.indentLevel = startIndent;
            return dirty;
        }

        private void DrawAround(Rect treeArea, Rect smallArea, Rect nodeArea)
        {
            Rect top = smallArea;
            top.y += 4;
            top.height = 3;
            GUIDrawRect(top, EditorResources.Instance.GetBackground().GetPixel(0, 0) * 0.7f);
            var bottom = top;
            bottom.y -= nodeArea.height;
            GUIDrawRect(bottom, EditorResources.Instance.GetBackground().GetPixel(0, 0) * 0.7f);
            Rect left = bottom;
            left.width = 3;
            left.height = nodeArea.height;
            left.x += 2 + ScrollPosition.x;
            var right = left;
            right.x += treeArea.width - 20 + ScrollPosition.x;

            GUIDrawRect(left, EditorResources.Instance.GetBackground().GetPixel(0, 0) * 0.7f);
            GUIDrawRect(right, EditorResources.Instance.GetBackground().GetPixel(0, 0) * 0.7f);
        }

        public void Filter(Func<T, bool> filter)
        {
            filterFunc = filter;
            triggerFilter = true;
        }

        public void FocusOnSelectedNode()
        {
            focusOnSelectedNode = true;
        }

        private bool genericCanPlaceHere(T p, T n)
        {
            if (p == null || n == null || p == n || n.IsRoot || TreeWalker.IsParentOf(n, p))
            {
                return false;
            }
            var actualparent = GetPotentialParent(p);
            if (TreeWalker.IsParentOf(n, actualparent))
                return false;

            if ((!p.IsFolder && !p.IsRoot) && n.IsFolder)
            {
                return false;
            }

            if (!CanPlaceHere(actualparent, n))
            {
                return false;
            }
            return true;
        }

        private T GetPotentialParent(T p)
        {
            if (p._getChildren.Any() && p.IsFoldedOut)
            {
                return p;
            }
            else
            {
                return p._getParent;
            }
        }


        private void genericPlaceHere(T p, T n)
        {
            InUndoHelper.DoInGroup(() =>
            {
                InUndoHelper.RecordObjects("Location", p, p._getParent, n, n._getParent);
                DeattachFromParent(n);
                if (p._getChildren.Any() && p.IsFoldedOut)
                {
                    AssignNewParent(p, n, 0);
                }
                else if (n._getParent == p._getParent)
                {
                    var index = p._getParent._getChildren.IndexOf(p);
                    AssignNewParent(p._getParent, n, index + 1);
                }
                else
                {
                    if (!p.IsRoot)
                    {
                        int newIndex = p._getParent._getChildren.IndexOf(p) + 1;
                        AssignNewParent(p._getParent, n, newIndex);
                    }
                    else
                    {
                        int newIndex = p._getChildren.IndexOf(p) + 1;
                        AssignNewParent(p, n, newIndex);
                    }
                }
            });

            
        }

        public static void GUIDrawRect(Rect position, Color color)
        {
            if (_staticRectStyle == null)
            {
                _staticRectStyle = new GUIStyle();
            }

            EditorResources.Instance.GenericColor.SetPixel(0, 0, color);
            EditorResources.Instance.GenericColor.Apply();

            _staticRectStyle.normal.background = EditorResources.Instance.GenericColor;

            GUI.Box(position, GUIContent.none, _staticRectStyle);
        }

        //Draw all nodes recursively 
        void DrawTree(T node, int indentLevel)
        {
            if (node != null)
            {
                if (node.IsFiltered)
                    return;
                EditorGUI.indentLevel = indentLevel + 1;

                bool clicked;
                //Draw node
                node.IsFoldedOut = OnNodeDraw(node, node == selectedNode, out clicked);


                Rect area = GUILayoutUtility.GetLastRect();

                Rect drawArea = area;
                drawArea.y += area.height - 5;
                drawArea.height = 10;

                toDrawArea.Add(new DrawArea(node, area, drawArea));

                if (node == selectedNode && focusOnSelectedNode && Event.current.type == EventType.Repaint)
                {
                    ScrollPosition.y = area.y - 50;
                    dirty = true;
                    focusOnSelectedNode = false;
                }

                if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                {
                    if (Event.current.Contains(drawArea) && DragAndDrop.objectReferences.Length > 0)
                    {
                        if (genericCanPlaceHere(node, DragAndDrop.objectReferences[0] as T))
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        }
                    }
                    else if (Event.current.Contains(area) && CanDropObjects(node, DragAndDrop.objectReferences))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    }
                }


                if (Event.current.type == EventType.DragPerform)
                {
                    requriesUpdate = true;
                    updateTime = Time.time;
                    if (Event.current.Contains(drawArea))
                    {
                        if (genericCanPlaceHere(node, DragAndDrop.objectReferences[0] as T))
                        {
                            genericPlaceHere(node, DragAndDrop.objectReferences[0] as T);
                        }
                        Event.current.UseEvent();
                    }
                    else if (Event.current.Contains(area) && CanDropObjects(node, DragAndDrop.objectReferences))
                    {
                        OnDrop(node, DragAndDrop.objectReferences);
                    }
                }

                if (area.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    {
                        DragAndDrop.PrepareStartDrag();
                        DragAndDrop.SetGenericData(node._ID.ToString(), new Object[] { node });
                        Event.current.UseEvent();

                    }
                    if (Event.current.type == EventType.MouseDrag)
                    {
                        var genericData = DragAndDrop.GetGenericData(node._ID.ToString()) as Object[];
                        if (genericData != null)
                        {
                            var data = genericData.FirstOrDefault() as T;
                            if (data != null && data == node)
                            {
                                DragAndDrop.objectReferences = new Object[] { node };
                                DragAndDrop.StartDrag("Dragging Node Element");
                                Event.current.UseEvent();
                            }
                        }
                    }
                }
                if (clicked)
                {
                    selectedNode = node;
                    GUIUtility.keyboardControl = 0;
                    Event.current.UseEvent();
                }

                if (Event.current.MouseUpWithin(area, 1))
                {

                    OnContext(node);

                    SelectedNode = node;
                }

                EditorGUI.indentLevel = indentLevel - 1;



                if (Event.current.type == EventType.Layout)
                    NodeWorker.RemoveNullChildren(node);

                if (node.IsFoldedOut)
                {
                    for (int i = 0; i < node._getChildren.Count; ++i)
                    {
                        T child = node._getChildren[i];
                        DrawTree(child, indentLevel + 1);
                    }
                }


            }
        }



        private void KeyboardControl()
        {
            #region keyboard control

            if (GUIUtility.keyboardControl != 0)
                return;

            if (Event.current.IsKeyDown(KeyCode.LeftArrow))
            {
                selectedNode.IsFoldedOut = false;
                FocusOnSelectedNode();
                Event.current.UseEvent();
            }
            if (Event.current.IsKeyDown(KeyCode.RightArrow))
            {
                selectedNode.IsFoldedOut = true;
                FocusOnSelectedNode();
                Event.current.UseEvent();
            }

            if (Event.current.IsKeyDown(KeyCode.UpArrow))
            {
                selectedNode = TreeWalker.FindPreviousUnfoldedNode(selectedNode, arg => !arg.IsFiltered);
                FocusOnSelectedNode();
                Event.current.UseEvent();
            }
            if (Event.current.IsKeyDown(KeyCode.DownArrow))
            {
                selectedNode = TreeWalker.FindNextNode(SelectedNode, arg => !arg.IsFiltered);
                FocusOnSelectedNode();
                Event.current.UseEvent();
            }
            if (Event.current.IsKeyDown(KeyCode.Home))
            {
                ScrollPosition = new Vector2();
            }
            if (Event.current.IsKeyDown(KeyCode.End))
            {
                ScrollPosition = new Vector2(0, 100000);
            }

            //    if (hasPressedDown && (_area.y + ScrollPosition.y + _area.height - selectedArea.height * 2 < selectedArea.y + selectedArea.height))
            //    {
            //        ScrollPosition.y += selectedArea.height;
            //    }

            //    if (hasPressedUp && (_area.y + ScrollPosition.y + selectedArea.height  > selectedArea.y))
            //    {
            //        ScrollPosition.y -= selectedArea.height;
            //    }
            #endregion
        }

        //FilterBy: true if node contains search
        private bool FilterNodes(T node, Func<T, bool> filter)
        {
            if (node == null)
                return false;
            node.IsFiltered = false;
            if (node._getChildren.Count > 0)
            {
                bool allChildrenFilted = true;
                foreach (var child in node._getChildren)
                {
                    bool filtered = FilterNodes(child, filter);
                    if (!filtered)
                    {
                        allChildrenFilted = false;
                    }
                }
                node.IsFiltered = allChildrenFilted; //If all children are filtered, this node also becomes filtered unless its name is not filtered
                if (node.IsFiltered)
                    node.IsFiltered = filter(node);
                return node.IsFiltered;
            }
            else
            {
                node.IsFiltered = filter(node);
                return node.IsFiltered;
            }
        }

        //Workaround to force redraw
        private void Update()
        {
            if (requriesUpdate && Window != null && updateTime + 0.5f > Time.time)
            {

                Window.Repaint();
                requriesUpdate = false;
            }
        }


        private bool dirty;
        private static GUIStyle _staticRectStyle;

        private EditorWindow Window;
        private bool requriesUpdate = false;
        private float updateTime = 0;

        private T selectedNode;
        private T draggingNode;
        private Rect selectedArea;

        private bool triggerFilter = false;
        private Func<T, bool> filterFunc;

        private bool canDropObjects;

        private Vector2 clickPos;

        private List<DrawArea> toDrawArea = new List<DrawArea>();

        private bool focusOnSelectedNode;

        private struct DrawArea
        {
            public Rect Big;
            public Rect Small;
            public T Node;

            public DrawArea(T node, Rect big, Rect small)
            {
                this.Node = node;
                Big = big;
                Small = small;
            }
        }
    }
}