using System;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

public class TreeDrawer<T> where T : UnityEngine.Object, InITreeNode<T>
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

    public delegate void OnDropDelegate(T node, UnityEngine.Object[] objects);
    public OnDropDelegate OnDrop;

    public delegate bool CanDropObjectsDelegate(T node, UnityEngine.Object[] objects);
    public CanDropObjectsDelegate CanDropObjects;

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

        EditorGUILayout.EndScrollView();

        EditorGUI.indentLevel = startIndent;
        return false;
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
            ////
            Rect area = GUILayoutUtility.GetLastRect();
            EditorGUI.indentLevel = indentLevel - 1;
            if (Event.current.DraggedWithin(area) && CanDropObjects(node, DragAndDrop.objectReferences))
            {
                DragHandle(node);
                Event.current.Use();
            }
            if (Event.current.MouseUpWithin(area, 1))
            {
                OnContext(node);
                Event.current.Use();
            }
            
            if (!node.IsFoldedOut)
                return;
            
            if(Event.current.type == EventType.Layout)
                NodeWorker.RemoveNullChildren(node);
            for (int i = 0; i < node._getChildren.Count; ++i)
            {
                T child = node._getChildren[i];
                DrawTree(child, indentLevel + 1);
            }
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