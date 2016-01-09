using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

public static class FolderDrawer
{
    public static void Draw(InAudioNode node)
    {
            var nodeData = node._nodeData;
        EditorGUILayout.BeginVertical();
            var data = node._nodeData as InFolderData;
         

        #region Mixer
        DataDrawerHelper.DrawMixer(node);
        #endregion
        EditorGUILayout.Separator();
        #region Volume
        if (Application.isPlaying)
        {
            InUndoHelper.GUIUndo(nodeData, "Folder volume", ref data.runtimeVolume, () => EditorGUILayout.Slider("Runtime Volume", data.runtimeVolume, 0, 1));
        }
        else
        {
            InUndoHelper.GUIUndo(nodeData, "Folder volume", ref data.VolumeMin, () => EditorGUILayout.Slider("Initial Volume", data.VolumeMin, 0, 1));
                
        }
        #endregion 

        EditorGUILayout.EndVertical();
    } 
}


}