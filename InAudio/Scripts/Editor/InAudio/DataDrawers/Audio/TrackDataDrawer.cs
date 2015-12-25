using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{
public static class TrackDataDrawer
{
    private static int selectedArea;
    private static Vector2 ScrollArea;
    //private static InAudioNode activeNode;
    private static InLayerData toRemove;
    public static void Draw(InAudioNode node)
    {
        //node.ScrollPosition = GUILayout.BeginScrollView(node.ScrollPosition);

        EditorGUILayout.BeginVertical();
        var trackData = (node._nodeData as InTrackData);
        NodeTypeDataDrawer.DrawName(node);

        //UndoHelper.GUIUndo(trackData, "Track length", ref trackData.TrackLength, () => EditorGUILayout.FloatField("Track length", trackData.TrackLength));
        
        
        selectedArea = GUILayout.SelectionGrid(selectedArea, new []{"Track", "Standard Settings"}, 2);
        EditorGUILayout.HelpBox("Hold control to drag a child node onto a track.", MessageType.None);

        if (selectedArea == 1)
        {
            NodeTypeDataDrawer.Draw(node);
        }
        else
        {
            EditorGUILayout.BeginVertical();
            ScrollArea = EditorGUILayout.BeginScrollView(ScrollArea, false, false);
            EditorGUILayout.BeginVertical();

            foreach (var layer in trackData.Layers)
            {
                DrawItem(node, layer);
            }

            if (GUILayout.Button("Add Layer", GUILayout.Width(150)))
            {
                InUndoHelper.RecordObjectFull(trackData, "Add layer");
                trackData.Layers.Add(new InLayerData());
            }

      
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
        if (toRemove != null)
        {
            if (trackData.Layers.Remove(toRemove))
            {
                GUI.FocusControl("none");
                InUndoHelper.RegisterUndo(trackData, "Removed Layer");
            }
        }
        //GUILayout.EndScrollView();
    }

    private static Vector2 area;

    private static float DrawItem(InAudioNode node, InLayerData item)
    {
        EditorGUILayout.BeginVertical(GUILayout.Height(220));
        var trackData = node._nodeData as InTrackData;

        
        EditorGUILayout.BeginHorizontal();

        
        EditorGUILayout.BeginVertical(GUILayout.Width(150));
        GUILayout.Label("Left");
        GUILayout.Label("Inside left");
        InUndoHelper.GUIUndo(trackData, "Zoom", ref item.Zoom, () => Math.Max(0, EditorGUILayout.FloatField("Zoom", item.Zoom)));
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Delete Layer"))
        {
            InUndoHelper.RecordObjectFull(trackData, "Remove layer");
            {
                toRemove = item;
            }
        }
        EditorGUILayout.Separator();
        EditorGUILayout.EndVertical();

        Rect dragArea = EditorGUILayout.BeginVertical();
        //GUILayout.Label("Right");
        item.ScrollPos = EditorGUILayout.BeginScrollView(item.ScrollPos, false, false);
        GUILayout.Label("Hello");
        Rect start = GUILayoutUtility.GetLastRect();
        var t = new Texture2D(1, 1);
        t.SetPixel(0,0, Color.white);
        GUI.DrawTexture(start, t, ScaleMode.StretchToFill);
        //start.y += 20;
        //GUI.Label(start, "Hello"); 

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        //EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        DropAreaGUI<InAudioNode>(dragArea, item, otherNode => !TreeWalker.IsParentOf(node, otherNode) || node == otherNode, AfterDrag);
        
        return 0;
    }

    private static void AfterDrag(InLayerData layer, InAudioNode inAudioNode, Vector2 position)
    { 
        Debug.Log("Ok");
    }

    private static bool Predicate(InAudioNode inAudioNode)
    {
        
      /*  if (!TreeWalker.IsParentOf(inAudioNode, activeNode))
        {
            return true;    
        }
        return false;
    */
        return false;
    }

    private static void DropAreaGUI<T>(Rect drop_area, InLayerData layer, Func<T, bool> predicate, Action<InLayerData, T, Vector2> afterDrag) where T : Object, InITreeNode<T>
    {
        Event evt = Event.current;

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:

                var item = DragAndDrop.objectReferences[0] as T;
                if (!drop_area.Contains(evt.mousePosition) || predicate(item))
                {
                    //DragAndDrop.visualMode = DragAndDropVisualMode.None;
                    return;
                }            

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    if (afterDrag != null)
                        afterDrag(layer, item, evt.mousePosition);
                    Event.current.UseEvent();
                }
                
                break;
        }
    }
}

}