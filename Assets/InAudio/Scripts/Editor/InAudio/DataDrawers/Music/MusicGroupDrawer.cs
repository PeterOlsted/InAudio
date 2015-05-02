using System;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.InAudioEditor;
using InAudioSystem.ReorderableList;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem
{
    public static class MusicGroupDrawer 
    {
        public static Vector2 scrollView;

        public static void Draw(InMusicGroup node)
        {
            node.ScrollPosition = EditorGUILayout.BeginScrollView(node.ScrollPosition);
            var prop = new SerializedObject(node);
            prop.Update();

            EditorGUIHelper.DrawID(node._guid);
            EditorGUILayout.PropertyField(prop.FindProperty("_name"));

            if(!Application.isPlaying)
                UndoHelper.GUIUndo(node, "Volume", ref node._minVolume, () => EditorGUILayout.Slider("Initial Volume",node._minVolume, 0f, 1f));
            else
                UndoHelper.GUIUndo(node, "Volume", ref node.runtimeVolume, () => EditorGUILayout.Slider("Current Volume", node.runtimeVolume, 0f, 1f));

            if (!Application.isPlaying)
                UndoHelper.GUIUndo(node, "Pitch", ref node._minPitch, () => EditorGUILayout.Slider("Initial Pitch", node._minPitch, 0f, 3f));
            else
                UndoHelper.GUIUndo(node, "Pitch", ref node.runtimePitch, () => EditorGUILayout.Slider("Current Pitch", node.runtimePitch, 0f, 3f));

            var playingInfo = node.PlayingInfo;
            if (playingInfo.Fading && Application.isPlaying)
            {
                EditorGUILayout.Slider("Node fading to", playingInfo.TargetVolume, 0f, 1f);
                GUI.enabled = false;
                var duration = playingInfo.EndTime - playingInfo.StartTime;
                var left = playingInfo.EndTime - Time.time;
                EditorGUILayout.Slider("Time elapsed", duration-left, 0, duration);
                GUI.enabled = true;
            }

            EditorGUILayout.PropertyField(prop.FindProperty("_loop"));

            EditorGUILayout.Separator();

            DataDrawerHelper.DrawMixer(node, prop.FindProperty("_mixerGroup"));
            EditorGUILayout.Separator();

            prop.ApplyModifiedProperties();

            if (!Application.isPlaying)
                UndoHelper.GUIUndo(node, "Mute", ref node._mute, () => EditorGUILayout.Toggle("Initial Mute", node._mute));
            else
                UndoHelper.GUIUndo(node, "Mute", ref node.runtimeMute, () => EditorGUILayout.Toggle("Currently Mute", node.runtimeMute));

            if (!Application.isPlaying)
                UndoHelper.GUIUndo(node, "Solo", ref node._solo, () => EditorGUILayout.Toggle("Initial Solo", node._solo));
            else
                UndoHelper.GUIUndo(node, "Solo", ref node.runtimeSolo, () => EditorGUILayout.Toggle("Currently Solo", node.runtimeSolo));

            EditorGUILayout.Separator();
            prop.Update();

            EditorGUILayout.LabelField("Clips in node");
            if (Application.isPlaying)
            {
                
                ReorderableListGUI.ListField(node._clips, (position, item) => DrawItem(node, prop, position, item),
                    ReorderableListFlags.DisableDuplicateCommand | ReorderableListFlags.DisableReordering);
            }
            else
            {
                ReorderableListGUI.ListField(node._clips, (position, item) => DrawItem(node, prop, position, item),
                    ReorderableListFlags.DisableDuplicateCommand);
            }
            if (prop.ApplyModifiedProperties())
            {
                //AudioBankWorker.RebuildBanks();
            }
            

            EditorGUILayout.Separator();
            if (node._children.Count > 0)
            {
                EditorGUILayout.LabelField("Child nodes");
                ReorderableListGUI.ListField(node._children, (position, item) =>
                {
                    EditorGUI.LabelField(position, item._name);
                    return item;
                },
                    ReorderableListFlags.DisableContextMenu | ReorderableListFlags.DisableReordering |
                    ReorderableListFlags.HideAddButton | ReorderableListFlags.HideRemoveButtons);
            }
            EditorGUILayout.EndScrollView();
        }

        private static AudioClip DrawItem(InMusicGroup node, SerializedObject obj, Rect position, AudioClip item)
        {
            var i = node._clips.FindIndex(item);

            var area = position;
            area.height -= 3;
            EditorGUI.PropertyField(area, obj.FindProperty("_clips").GetArrayElementAtIndex(i));
            area.y += area.height;
            area.height = 3;
            if (item != null && Application.isPlaying)
            {
                var player = node.PlayingInfo.Players.Find(s => s.clip == item);
                if (player != null)
                {
                    EditorGUI.DrawRect(area, Color.white);
                    float pos = (float) player.ExactPosition();
                    area.width = area.width*Mathf.Clamp01(pos/item.length);
                    EditorGUI.DrawRect(area, Color.green);
                }
            }
            return obj.FindProperty("_clips").GetArrayElementAtIndex(i).objectReferenceValue as AudioClip;
        }
    }
}