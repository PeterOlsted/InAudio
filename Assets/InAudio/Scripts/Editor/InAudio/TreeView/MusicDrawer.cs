using InAudioSystem.ExtensionMethods;
using InAudioSystem.InAudioEditor;
using InAudioSystem.Internal;
using InAudioSystem.Runtime;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.TreeDrawer
{
    public class MusicDrawer
    {

        public static bool Draw(InMusicNode node, bool isSelected, out bool clicked) 
        {
            var group = node as InMusicGroup;
            clicked = false;

            Rect fullArea = EditorGUILayout.BeginHorizontal();

            Rect area = EditorGUILayout.BeginHorizontal();
            if (isSelected)
                GUI.DrawTexture(area, EditorResources.Background);

            GUILayout.Space(EditorGUI.indentLevel * 16);

            if (group != null)
            {
                DrawVolume(fullArea, group);
            }

            bool folded = node.IsFoldedOut;

            Texture picture;
            if (folded || node._getChildren.Count == 0)
                picture = EditorResources.Minus;
            else
                picture = EditorResources.Plus;

            if (GUILayout.Button(picture, GUIStyle.none, GUILayout.Height(EditorResources.Minus.height),
                GUILayout.Width(EditorResources.Minus.width)))
            {
                node.IsFoldedOut = !node.IsFoldedOut;
                EditorEventUtil.UseEvent();
            }

            Texture icon = LookUpIcon(node);

            TreeNodeDrawerHelper.DrawIcon(GUILayoutUtility.GetLastRect(), icon, GUIStyle.none);
            EditorGUILayout.LabelField("");

            EditorGUILayout.EndHorizontal();
            Rect labelArea = GUILayoutUtility.GetLastRect();
            labelArea.y += 8;
            labelArea.x += 60;
            EditorGUI.LabelField(labelArea, node.GetName);



            if (group != null)
            {
                if (!Application.isPlaying)
                    GUI.enabled = false;
                DrawPlayStop(fullArea, group);
                GUI.enabled = true;
                DrawMuteSolo(fullArea, group);
            }


            EditorGUILayout.EndHorizontal();
            if (Event.current.ClickedWithin(fullArea, 0))
            {
                clicked = true;
            }
         
            return node.IsFoldedOut;
        }

        private static void DrawPlayStop(Rect fullArea, InMusicGroup group)
        {
            Rect butArea = fullArea;
            butArea.width = 16;
            butArea.height = 12;
            butArea.y += 3;
            butArea.x += EditorGUI.indentLevel * 5 - 5;

            Texture playing;
            
            if(Application.isPlaying)
                playing = group.PlayingInfo.State == MusicState.Playing ? EditorResources.Pause : EditorResources.Play;
            else
            {
                playing = EditorResources.Play;
            }

            if (GUI.Button(butArea, playing, GUIStyle.none))
            {
                if(group.PlayingInfo.State == MusicState.Playing)
                    InAudio.Music.Pause(group);
                else
                    InAudio.Music.Play(group);
            }


            if (group.PlayingInfo.State == MusicState.Stopped || group.PlayingInfo.State == MusicState.Nothing)
            {
                GUI.enabled = false;
            }
            butArea.y += 14;
            Texture stop= EditorResources.Stop;

            if (GUI.Button(butArea, stop, GUIStyle.none))
            {
                InAudio.Music.Stop(group);
            }
            GUI.enabled = true;
        }

        private static void DrawVolume(Rect fullArea, InMusicGroup @group)
        {
            GUI.enabled = false;
            Rect sliderRect = fullArea;
            sliderRect.x = sliderRect.width - 30;
            sliderRect.width = 20;
            sliderRect.height -= 5;
            
            GUI.VerticalSlider(sliderRect, @group.PlayingInfo.FinalVolume, 1f, 0f);

            GUI.enabled = true;
        }

        private static void DrawMuteSolo(Rect fullArea, InMusicGroup group)
        {
            Rect butArea = fullArea;
            butArea.width = 16;
            butArea.height = 12;
            butArea.y += 3;
            butArea.x += EditorGUI.indentLevel*5 + 10;

            Texture mute = MusicVolumeUpdater.IsMute(group) ? EditorResources.Muted : EditorResources.NotMute;

            if (GUI.Button(butArea, mute, GUIStyle.none))
            {
                UndoHelper.RegisterUndo(group, "Mute");
                MusicVolumeUpdater.FlipMute(group);
            }

            butArea.y += 14;
            Texture solo = MusicVolumeUpdater.IsSolo(group) ? EditorResources.Soloed : EditorResources.NotSolo;

            if (GUI.Button(butArea, solo, GUIStyle.none))
            {
                UndoHelper.RegisterUndo(group, "Solo");
                MusicVolumeUpdater.FlipSolo(group);
            }
        }

        public static Texture LookUpIcon(InMusicNode node)
        {

             var group = node as InMusicGroup;
            if (!Application.isPlaying || group == null)
            {
                if (node._type == MusicNodeType.Music)
                    return EditorResources.Audio;
                if (node.IsRootOrFolder)
                    return EditorResources.Folder;
                return null;
            }
            else
            {
                if (group.PlayingInfo.State == MusicState.Playing)
                    return EditorResources.Play;
                else if (group.PlayingInfo.State == MusicState.Paused)
                    return EditorResources.Pause;
                else //if (group.PlayingInfo.State == MusicState.Stopped)
                    return EditorResources.Stop;
            }
        }
    }

}
