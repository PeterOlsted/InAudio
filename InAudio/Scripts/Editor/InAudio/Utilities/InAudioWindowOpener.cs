using System;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class InAudioWindowOpener {

        [MenuItem("Window/InAudio/Audio Window #&1", false, 1)]
        public static InAudioWindow ShowAudioWindow()
        {
            return InAudioWindow.Launch();
        }

        [MenuItem("Window/InAudio/Music Window #&2", false, 2)]
        public static InMusicWindow ShowMusicWindow()
        {
            return InMusicWindow.Launch();
        }

        [MenuItem("Window/InAudio/Event Window #&3", false, 3)]
        public static EventWindow ShowEventWindow()
        {
            return EventWindow.Launch();
        }

        public static void ShowNewDataWindow(Action<GameObject> callback)
        {
            SplitDataWindow window = EditorWindow.GetWindow<SplitDataWindow>();
            window.AssignCallback(callback);
        }

        [MenuItem("Window/InAudio/Banks Window #&4", false, 4)]
        public static AuxWindow ShowBanksWindow()
        {
            AuxWindow.Launch();
            AuxWindow window = EditorWindow.GetWindow<AuxWindow>();
            if (window != null)
            {
                window.SelectBankCreation();
            }
            return window;
        }

        [MenuItem("Window/InAudio/Integrity Window #&5", false, 5)]
        public static AuxWindow ShowIntegrityWindow()
        {
            AuxWindow.Launch();
            AuxWindow window = EditorWindow.GetWindow(typeof(AuxWindow)) as AuxWindow;
            if (window != null) 
            {
                window.SelectIntegrity();
            }
            return window;
        }

     
        [MenuItem("Window/InAudio/Introduction Window #&6", false, 6)]
        public static GuideWindow ShowIntroductionWindow()
        {
            GuideWindow window = EditorWindow.GetWindow<GuideWindow>();

            window.Show();
            window.SetTitle("InAudio Introduction");
            return window;
        }

        //[MenuItem("Window/InAudio/Music Settings#&6", false, 7)]
        //private static void InteractiveMusic()
        //{
        //    InteractiveMusicWindow window = EditorWindow.GetWindow<InteractiveMusicWindow>();

        //    window.Show();
        //    window.minSize = new Vector2(100, 50);
        //    window.title = "Music Settings";
        //}



        [MenuItem("Window/InAudio/Documentation (website)", false, 10)]
        private static void ShowDocumentation()
        {
            Application.OpenURL("http://innersystems.net/wiki");
        }

        [MenuItem("Window/InAudio/Forum (website)", false, 11)]
        private static void ShowForum()
        {
            Application.OpenURL("http://forum.unity3d.com/threads/232490-InAudio-Advanced-Audio-for-Unity");
        }


        [MenuItem("GameObject/Audio/InAudio Spline")]
        private static void CreateSpline()
        {
            GameObject go = new GameObject("Audio Spline");
            go.AddComponent<InSpline>();
            Selection.activeGameObject = go;
        }
    }

    public static class EditorWindowExtensions
    {
        public static void SetTitle(this EditorWindow window, string title)
        {
#if UNITY_5_0
            window.title = title;
#else
            window.titleContent = new GUIContent(title);
#endif
        }
    }
}

