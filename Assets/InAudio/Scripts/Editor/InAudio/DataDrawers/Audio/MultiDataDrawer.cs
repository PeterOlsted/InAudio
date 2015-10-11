using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{ 
public static class MultiDataDrawer  {
    public static void Draw(InAudioNode node)
    {
        node.ScrollPosition = GUILayout.BeginScrollView(node.ScrollPosition);
        InUndoHelper.GUIUndo(node, "Name Change", () =>
            EditorGUILayout.TextField("Name", node.Name),
            s => node.Name = s);
        NodeTypeDataDrawer.Draw(node); 
        GUILayout.EndScrollView();
    }
}
}