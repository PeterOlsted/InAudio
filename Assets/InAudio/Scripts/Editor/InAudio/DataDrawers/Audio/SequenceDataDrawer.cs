using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
public static class SequenceDataDrawer
{
    public static void Draw(InAudioNode node)
    {
        node.ScrollPosition = GUILayout.BeginScrollView(node.ScrollPosition);
        InUndoHelper.GUIUndo(node, "Name Change", ref node.Name, () =>
            EditorGUILayout.TextField("Name", node.Name));
        NodeTypeDataDrawer.Draw(node);
        GUILayout.EndScrollView();
    }
}
}