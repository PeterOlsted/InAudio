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
        private static Object lastClickNode;
        private static GUIStyle noMargain;
        public static bool Draw(InMusicNode node, bool isSelected, out bool clicked) 
        {
            var group = node as InMusicGroup;
            clicked = false;
            if (noMargain == null)
            {
                noMargain = new GUIStyle();
                noMargain.margin = new RectOffset(0, 0, 0, 0);
            }
            Rect fullArea = EditorGUILayout.BeginHorizontal();

            Rect area = EditorGUILayout.BeginHorizontal();
            if (isSelected)
                GUI.DrawTexture(area, EditorResources.Background);

            GUILayout.Space(EditorGUI.indentLevel * 16);

            bool folded = node.IsFoldedOut;

            Texture picture;
            if (folded || node._getChildren.Count == 0)
                picture = EditorResources.Minus;
            else
                picture = EditorResources.Plus;

            GUILayout.Label(picture, noMargain, GUILayout.Height(EditorResources.Minus.height),
                GUILayout.Width(EditorResources.Minus.width));
            Rect foldRect = GUILayoutUtility.GetLastRect();
            if (Event.current.ClickedWithin(foldRect))
            {
                node.IsFoldedOut = !node.IsFoldedOut;
                Event.current.Use();
            }

            Texture icon = LookUpIcon(node);
           


            TreeNodeDrawerHelper.DrawIcon(GUILayoutUtility.GetLastRect(), icon, noMargain);
            EditorGUILayout.LabelField("");


            EditorGUILayout.EndHorizontal();
            Rect labelArea = GUILayoutUtility.GetLastRect();
            Rect buttonArea = labelArea;
            if (!node.IsRoot)
            {
                buttonArea.x = buttonArea.x + 56 + EditorGUI.indentLevel * 16;
                buttonArea.width = 20;
                buttonArea.height = 14;
                GUI.Label(buttonArea, EditorResources.Up, noMargain);
                if (Event.current.ClickedWithin(buttonArea))
                {
                    NodeWorker.MoveNodeOneUp(node);
                    Event.current.Use();
                }
                buttonArea.y += 15;
                GUI.Label(buttonArea, EditorResources.Down, noMargain);
                if (Event.current.ClickedWithin(buttonArea))
                {
                    NodeWorker.MoveNodeOneDown(node);
                    Event.current.Use();
                }
                labelArea.x += 20;
            }
            labelArea.y += 6;
            labelArea.x += 65;
            EditorGUI.LabelField(labelArea, node.GetName);

          

            if (group != null)
            {
                if (!Application.isPlaying)
                    GUI.enabled = false;
                DrawPlayStop(fullArea, group);
                GUI.enabled = true;
                DrawMuteSolo(fullArea, group);
            }

            if (Event.current.ClickedWithin(fullArea, 0))
            {
                lastClickNode = node;
                clicked = true;
            }
            if (Event.current.type == EventType.MouseDrag && isSelected && lastClickNode == node && Event.current.button == 0 && fullArea.Contains(Event.current.mousePosition) && DragAndDrop.objectReferences.Length == 0)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = new UnityEngine.Object[] { node };
                DragAndDrop.StartDrag("Music Node Drag");
                Event.current.Use();
            }

            
            if (group != null)
            {
                DrawVolume(fullArea, @group);
            }
            EditorGUILayout.EndHorizontal();

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

            if (GUI.Button(butArea, playing, noMargain))
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

            if (GUI.Button(butArea, stop, noMargain))
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
            //if (Application.isPlaying)
            {
                GUI.VerticalSlider(sliderRect, @group.PlayingInfo.FinalVolume, 1f, 0f);
            }
            //else
            {
                //var parent = group.Parent as InMusicGroup;
                //float pVolume = 1f;
                //if (parent != null)
                //{
                //    pVolume = group._minVolume * pVolume.
                //}
                //GUI.VerticalSlider(sliderRect, pVolume, 1f, 0f);
            }
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

            if (GUI.Button(butArea, mute, noMargain))
            {
                UndoHelper.RegisterUndo(group, "Mute");
                MusicVolumeUpdater.FlipMute(group);
            }


            butArea.y += 14;
            Texture solo = MusicVolumeUpdater.IsSolo(group) ? EditorResources.Soloed : EditorResources.NotSolo;

            if (GUI.Button(butArea, solo, noMargain))
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
