using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class MusicFolderDrawer
    {
        public static void Draw(InMusicFolder node)
        {
            node.ScrollPosition = EditorGUILayout.BeginScrollView(node.ScrollPosition);
            var prop = new SerializedObject(node);
            prop.Update();
            EditorGUILayout.BeginVertical();
            #region Mixer

            DataDrawerHelper.DrawMixer(node, prop.FindProperty("_mixerGroup"));

            #endregion
            EditorGUILayout.Separator();
            #region Volume & Pitch
            if (!Application.isPlaying)
                InUndoHelper.GUIUndo(node, "Volume", ref node._minVolume, () => EditorGUILayout.Slider("Initial Volume", node._minVolume, 0f, 1f));
            else
                InUndoHelper.GUIUndo(node, "Volume", ref node.runtimeVolume, () => EditorGUILayout.Slider("Current Volume", node.runtimeVolume, 0f, 1f));

            if (!Application.isPlaying)
                InUndoHelper.GUIUndo(node, "Pitch", ref node._minPitch, () => EditorGUILayout.Slider("Initial Pitch", node._minPitch, 0f, 3f));
            else
                InUndoHelper.GUIUndo(node, "Pitch", ref node.runtimePitch, () => EditorGUILayout.Slider("Current Pitch", node.runtimePitch, 0f, 3f));
            #endregion

            EditorGUILayout.EndVertical();
            prop.ApplyModifiedProperties();

            EditorGUILayout.EndScrollView();
        }
    }
}