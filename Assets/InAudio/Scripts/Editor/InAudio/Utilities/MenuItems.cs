using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class MenuItems {

        [MenuItem("Window/InAudio/Audio Window #&1", false, 1)]
        private static void ShowAudioWindow()
        {
            InAudioWindow.Launch();
        }

        [MenuItem("Window/InAudio/Music Window #&2", false, 2)]
        private static void ShowMusicWindow()
        {
            InMusicWindow.Launch();
        }

        [MenuItem("Window/InAudio/Event Window #&3", false, 3)]
        private static void ShowEventWindow()
        {
            EventWindow.Launch();
        }

        [MenuItem("Window/InAudio/Banks Window #&4", false, 4)]
        private static void ShowBanksWindow()
        {
            AuxWindow.Launch();
            AuxWindow window = EditorWindow.GetWindow(typeof(AuxWindow)) as AuxWindow;
            if (window != null)
            {
                window.SelectBankCreation();
            }
        }

        [MenuItem("Window/InAudio/Integrity Window #&5", false, 5)]
        private static void ShowIntegrityWindow()
        {
            AuxWindow.Launch();
            AuxWindow window = EditorWindow.GetWindow(typeof(AuxWindow)) as AuxWindow;
            if (window != null) 
            {
                window.SelectIntegrity();
            }
             
        }

        [MenuItem("Window/InAudio/Feedback Window #&6", false, 6)]
        private static void ShowFeedbackWindow()
        {
            FeedbackWindow window = EditorWindow.GetWindow<FeedbackWindow>();

            window.Show();
            window.minSize = new Vector2(100, 50);
            window.SetTitle("Feedback");
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

