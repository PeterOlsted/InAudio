using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class MenuItems {

        [MenuItem("Window/InAudio/Audio Window #&1")]
        private static void ShowAudioWindow()
        {
            InAudioWindow.Launch();
        }

        [MenuItem("Window/InAudio/Music Window #&2")]
        private static void ShowMusicWindow()
        {
            InMusicWindow.Launch();
        }

        [MenuItem("Window/InAudio/Event Window #&3")]
        private static void ShowEventWindow()
        {
            EventWindow.Launch();
        }

        [MenuItem("Window/InAudio/Banks Window #&4")]
        private static void ShowBanksWindow()
        {
            AuxWindow.Launch();
            AuxWindow window = EditorWindow.GetWindow(typeof(AuxWindow)) as AuxWindow;
            if (window != null)
            {
                window.SelectBankCreation();
            }
        }

        [MenuItem("Window/InAudio/Integrity Window #&5")]
        private static void ShowIntegrityWindow()
        {
            AuxWindow.Launch();
            AuxWindow window = EditorWindow.GetWindow(typeof(AuxWindow)) as AuxWindow;
            if (window != null)
            {
                window.SelectIntegrity();
            }

        }


        [MenuItem("Window/InAudio/Documentation (website)")]
        private static void ShowDocumentation()
        {
            Application.OpenURL("http://innersystems.net/wiki");
        }

        [MenuItem("Window/InAudio/Forum (website)")]
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
    
}

