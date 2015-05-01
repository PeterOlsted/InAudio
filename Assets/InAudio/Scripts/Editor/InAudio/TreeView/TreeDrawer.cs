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

    private bool genericCanPlaceHere(T p, T n)
    {
        if (p == null || n == null || p == n || n.IsRoot || TreeWalker.IsParentOf(n, p))
        {
            return false;
        }
        if ((p.IsFolder || p.IsRoot) && !n.IsFolder)
        {
            return false;
        }
        if (n.IsFolder && !p.IsFolder && !p.IsRoot)
        {
            return false;
        }
        if (!CanPlaceHere(p, n))
        {
            return false;
        }
        return true;
    }

    public delegate void PlaceHereDelegate(T newParent, T toPlace);

    private void genericPlaceHere (T p, T n)
    {
//        Debug.Log(p.GetName+ " " + n.GetName); 

        UndoHelper.RecordObjects("Location", p, p._getParent, n, n._getParent);

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
            int newIndex = p._getParent._getChildren.IndexOf(p) + 1;
            AssignNewParent(p._getParent, n, newIndex);
        }
    }



    private T selectedNode;
    private T draggingNode;
    private Rect selectedArea;

    private bool triggerFilter = false;
    private Func<T, bool> filterFunc;

    private bool canDropObjects;

    private Vector2 clickPos;

    public bool DrawTree(T treeRoot, Rect area)
    {
        if (SelectedNode == null)
            selectedNode = treeRoot;

        if (SelectedNode == null) //If it's still null
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
            //IsDirty = true;
        }


        DrawTree(treeRoot, EditorGUI.indentLevel);
        

        foreach (var rect in toDrawArea)
        {
            if (rect.Area.Contains(Event.current.mousePosition))
            {
                GUIDrawRect(rect.Area, Color.red);
                
            }
        }
        if (Event.current.type == EventType.repaint)
        {
            toDrawArea.Clear();
        }



        EditorGUILayout.EndScrollView();
        
        EditorGUI.indentLevel = startIndent;
        return false;
    }

    private static Texture2D _staticRectTexture;
    private static GUIStyle _staticRectStyle;

    public static void GUIDrawRect(Rect position, Color color)
    {
        if (_staticRectTexture == null)
        {
            _staticRectTexture = new Texture2D(1, 1);
        }

        if (_staticRectStyle == null)
        {
            _staticRectStyle = new GUIStyle();
        }

        _staticRectTexture.SetPixel(0, 0, color);
        _staticRectTexture.Apply();

        _staticRectStyle.normal.background = _staticRectTexture;

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
            if (clicked)
                selectedNode = node;

            Rect area = GUILayoutUtility.GetLastRect();

            Rect drawArea = area;
            if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
            {
                drawArea.y += area.height-5;
                drawArea.height = 10;  
                toDrawArea.Add(new DrawTuple(drawArea, node));                
            }

            if (Event.current.type == EventType.dragUpdated && Event.current.Contains(area) && genericCanPlaceHere(node, DragAndDrop.objectReferences[0] as T))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
            }

            
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && area.Contains(Event.current.mousePosition) && DragAndDrop.objectReferences.Length == 0)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = new Object[] { node };
                DragAndDrop.StartDrag("Music Node Drag");
                Event.current.Use();
            }

            if (Event.current.Contains(drawArea) && Event.current.type == EventType.DragPerform && genericCanPlaceHere(node, DragAndDrop.objectReferences[0] as T))
            {
//                Debug.LogWarning("------------" + node.GetName);
                genericPlaceHere(node, DragAndDrop.objectReferences[0] as T);
                Event.current.Use();
            }

            EditorGUI.indentLevel = indentLevel - 1;

            if (Event.current.DraggedWithin(area) && CanDropObjects(node, DragAndDrop.objectReferences))
            {
                DragHandle(node);
            }

            if (Event.current.MouseUpWithin(area, 1))
            {
                OnContext(node);
                Event.current.Use();
            }
            
            if(Event.current.type == EventType.Layout)
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

    private List<DrawTuple> toDrawArea = new List<DrawTuple>();

    private struct DrawTuple
    {
        public Rect Area;
        public T Node;

        public DrawTuple(Rect area, T node)
        {
            Area = area;
            Node = node;
        }
    }

    private void KeyboardControl()
    {
        #region keyboard control

        if (GUIUtility.keyboardControl != 0)
            return;

        if (Event.current.IsKeyDown(KeyCode.UpArrow))
        {
            selectedNode = TreeWalker.FindPreviousUnfoldedNode(selectedNode, arg => !arg.IsFiltered);
            Event.current.Use();
        }
        if (Event.current.IsKeyDown(KeyCode.DownArrow))
        {
            selectedNode = TreeWalker.FindNextNode(SelectedNode, arg => !arg.IsFiltered );
            Event.current.Use();
        }
    //    if (Event.current.IsKeyDown(KeyCode.Home))
    //    {
    //        ScrollPosition = new Vector2();
    //        selectedNode = root;
    //    }
    //    if (Event.current.IsKeyDown(KeyCode.End))
    //    {
    //        //selectedNode = TreeWalker.;
    //    }

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
            if(node.IsFiltered)
                node.IsFiltered = filter(node);
            return node.IsFiltered;
        }
        else
        {
            node.IsFiltered = filter(node);
            return node.IsFiltered;
        }
    }

    public void Filter(Func<T, bool> filter)
    {
        filterFunc = filter;
        triggerFilter = true;
    }

    

    private void DragHandle(T node)
    {
        if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
        {  
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

            if (Event.current.type == EventType.DragPerform)
            {
                OnDrop(node, DragAndDrop.objectReferences);
            }
        }
    }

    public void FocusOnSelectedNode()
    {
        //focusOnSelectedNode = true;
    }
}
}