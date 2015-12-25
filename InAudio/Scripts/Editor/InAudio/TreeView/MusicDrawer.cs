using InAudioSystem.ExtensionMethods;
using InAudioSystem.InAudioEditor;
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
                GUI.DrawTexture(area, EditorResources.Instance.GetBackground());

            GUILayout.Space(EditorGUI.indentLevel * 16);

            
            DrawVolume(fullArea, node);
            

            bool folded = node.IsFoldedOut;

            Texture picture;
            if (folded || node._getChildren.Count == 0)
                picture = EditorResources.Instance.Minus;
            else
                picture = EditorResources.Instance.Plus;

            if (GUILayout.Button(picture, GUIStyle.none, GUILayout.Height(EditorResources.Instance.Minus.height), GUILayout.Width(EditorResources.Instance.Minus.width)))
            {
                node.IsFoldedOut = !node.IsFoldedOut;
                Event.current.UseEvent();
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
            }
            DrawMuteSolo(fullArea, node);


            EditorGUILayout.EndHorizontal();
            if (Event.current.ClickedWithin(fullArea))
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
            butArea.x += EditorGUI.indentLevel * 5 - 10;

            Texture playing;
            
            if(Application.isPlaying)
                playing = group.PlayingInfo.State == MusicState.Playing ? EditorResources.Instance.Pause : EditorResources.Instance.Play;
            else
            {
                playing = EditorResources.Instance.Play;
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
            Texture stop= EditorResources.Instance.Stop;

            if (GUI.Button(butArea, stop, GUIStyle.none))
            {
                InAudio.Music.Stop(group);
            }
            GUI.enabled = true;
        }

        private static void DrawVolume(Rect fullArea, InMusicNode @group)
        {
            GUI.enabled = false;
            Rect sliderRect = fullArea;
            sliderRect.x = sliderRect.width - 30;
            sliderRect.width = 20;
            sliderRect.height -= 5;
            
            GUI.VerticalSlider(sliderRect, @group.PlayingInfo.FinalVolume, 1f, 0f);

            GUI.enabled = true;
        }

        private static void DrawMuteSolo(Rect fullArea, InMusicNode node)
        {
            Rect butArea = fullArea;
            butArea.width = 16;
            butArea.height = 12;
            butArea.y += 3;
            butArea.x += EditorGUI.indentLevel*5 + 5;

            Texture mute = MusicUpdater.IsMute(node) ? EditorResources.Instance.Muted : EditorResources.Instance.NotMute;

            if (GUI.Button(butArea, mute, GUIStyle.none))
            {
                InUndoHelper.RegisterUndo(node, "Mute");
                MusicUpdater.FlipMute(node);
            }

            butArea.y += 14;
            Texture solo = MusicUpdater.IsSolo(node) ? EditorResources.Instance.Soloed : EditorResources.Instance.NotSolo;

            if (GUI.Button(butArea, solo, GUIStyle.none))
            {
                InUndoHelper.RegisterUndo(node, "Solo");
                MusicUpdater.FlipSolo(node);
            }
        }

        public static Texture LookUpIcon(InMusicNode node)
        {
             var group = node as InMusicGroup;
            if (!Application.isPlaying || group == null)
            {
                if (node._type == MusicNodeType.Music)
                    return EditorResources.Instance.Audio;
                if (node.IsRootOrFolder)
                    return EditorResources.Instance.Folder;
                return null;
            }
            else
            {
                if (group.PlayingInfo.State == MusicState.Playing)
                    return EditorResources.Instance.Play;
                else if (group.PlayingInfo.State == MusicState.Paused)
                    return EditorResources.Instance.Pause;
                else //if (group.PlayingInfo.State == MusicState.Stopped)
                    return EditorResources.Instance.Stop;
            }
        }
    }

}
