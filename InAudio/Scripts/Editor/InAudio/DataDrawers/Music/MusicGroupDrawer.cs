using InAudioSystem.ExtensionMethods;
using InAudioSystem.ReorderableList;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public static class MusicGroupDrawer 
    {
        public static Vector2 scrollView;

        public static void Draw(InMusicGroup node)
        {
            node.ScrollPosition = EditorGUILayout.BeginScrollView(node.ScrollPosition);
            var prop = new SerializedObject(node);
            prop.Update();

            EditorGUILayout.PropertyField(prop.FindProperty("_name"));
            

            if(!Application.isPlaying)
                InUndoHelper.GUIUndo(node, "Volume", ref node._minVolume, () => EditorGUILayout.Slider("Initial Volume",node._minVolume, 0f, 1f));
            else
                InUndoHelper.GUIUndo(node, "Volume", ref node.runtimeVolume, () => EditorGUILayout.Slider("Current Volume", node.runtimeVolume, 0f, 1f));

            if (!Application.isPlaying)
                InUndoHelper.GUIUndo(node, "Pitch", ref node._minPitch, () => EditorGUILayout.Slider("Initial Pitch", node._minPitch, 0f, 3f));
            else
                InUndoHelper.GUIUndo(node, "Pitch", ref node.runtimePitch, () => EditorGUILayout.Slider("Current Pitch", node.runtimePitch, 0f, 3f));
            EditorGUILayout.BeginHorizontal();
            if (Application.isPlaying)
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Save volume & pitch"))
                {
                    InUndoHelper.RecordObject(node, "Volume & Pitch for Music");
                    node._minVolume = node.runtimeVolume;
                    node._minPitch = node.runtimePitch;
                    Debug.Log("InAudio: Saved volume: "+node._minVolume + ", pitch: " + node._minPitch);
                    
                }
            }
            EditorGUILayout.EndHorizontal();

            var playingInfo = node.PlayingInfo;
            if (playingInfo.Fading && Application.isPlaying)
            {
                EditorGUILayout.Slider("Volume target", playingInfo.TargetVolume, 0f, 1f);
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
                InUndoHelper.GUIUndo(node, "Mute", ref node._mute, () => EditorGUILayout.Toggle("Initial Mute", node._mute));
            else
                InUndoHelper.GUIUndo(node, "Mute", ref node.runtimeMute, () => EditorGUILayout.Toggle("Currently Mute", node.runtimeMute));

            if (!Application.isPlaying)
                InUndoHelper.GUIUndo(node, "Solo", ref node._solo, () => EditorGUILayout.Toggle("Initial Solo", node._solo));
            else
                InUndoHelper.GUIUndo(node, "Solo", ref node.runtimeSolo, () => EditorGUILayout.Toggle("Currently Solo", node.runtimeSolo));

            EditorGUILayout.Separator();
            prop.Update();

            EditorGUILayout.LabelField("Clips in node");
            if (Application.isPlaying)
            {

                ReorderableListGUI.ListField(prop.FindProperty("_clips"), ReorderableListFlags.DisableDuplicateCommand | ReorderableListFlags.DisableContextMenu | ReorderableListFlags.DisableReordering | ReorderableListFlags.HideRemoveButtons);
                var rect = GUILayoutUtility.GetLastRect();
                for (int i = 0; i < node._clips.Count; i++)
                {
                    var progress = rect;
                    progress.height = 3;
                    progress.x += 5;
                    progress.width -= 30;
                    progress.y += i*20+20;
                    //GUI.DrawTexture(progress, EditorResources.Background);
                    var item = node._clips[i];
                    var player = node.PlayingInfo.Players.Find(s => s.clip == item);
                    if (player != null)
                    {
                        EditorGUI.DrawRect(progress, Color.white);
                        float pos = (float) player.ExactPosition();
                        if (node.PlayingInfo.State == MusicState.Playing || node.PlayingInfo.State == MusicState.Paused)
                        {
                            progress.width = progress.width*Mathf.Clamp01(pos/item.length);
                            EditorGUI.DrawRect(progress, Color.green);
                        }
                    }
                    
                }
                
            }
            else
            {
                ReorderableListGUI.ListField(prop.FindProperty("_clips"), ReorderableListFlags.DisableDuplicateCommand);
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

        //private static AudioClip DrawItem(InMusicGroup node, SerializedObject obj, Rect position, AudioClip item)
        //{
        //    var i = node._clips.FindIndex(item);
        //    position.width -= 30;
        //    Rect removalButton = position;
        //    removalButton.x += position.width;
        //    removalButton.width = 25;
            


        //    var area = position;
        //    area.height -= 3;
        //    EditorGUI.PropertyField(area, new SerializedObject(item));
        //    area.y += area.height;
        //    area.height = 3;
            //if (item != null && Application.isPlaying)
            //{
            //    var player = node.PlayingInfo.Players.Find(s => s.clip == item);
            //    if (player != null)
            //    {
            //        EditorGUI.DrawRect(area, Color.white);
            //        float pos = (float) player.ExactPosition();
            //        area.width = area.width*Mathf.Clamp01(pos/item.length);
            //        EditorGUI.DrawRect(area, Color.green);
            //    }
            //}

        //    var index = obj.FindProperty("_clips").GetArrayElementAtIndex(i).objectReferenceValue as AudioClip;
        //    if (GUI.Button(removalButton, "X"))
        //    {
        //        obj.FindProperty("_clips").DeleteArrayElementAtIndex(i);
        //    }

        //    return index;
        //}
    }
}