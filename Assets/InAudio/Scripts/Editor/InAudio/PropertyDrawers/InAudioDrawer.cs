using UnityEditor;
using UnityEngine;
using System.Collections;

namespace InAudioSystem
{

    [CustomEditor(typeof (InAudio))]
    public class InAudioDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Support");
            EditorGUILayout.SelectableLabel("inaudio@outlook.com");
            EditorGUILayout.EndHorizontal();


            GUI.enabled = Application.isPlaying;
            if (GUILayout.Button("Stop All Sound Effects"))
            {
                InAudio.StopAll();
            }
            if (GUILayout.Button("Stop All Music"))
            {
                InAudio.Music.StopAll();
            }
            GUI.enabled = true;
            EditorGUILayout.Separator();

            if (GUILayout.Button("Documentation"))
            {
                Application.OpenURL("http://innersystems.net/wiki");
            }
            if (GUILayout.Button("Forum"))
            {
                Application.OpenURL("http://forum.unity3d.com/threads/232490-InAudio-Advanced-Audio-for-Unity");
            }
            if (GUILayout.Button("Website"))
            {
                Application.OpenURL("http://innersystems.net/");
            }
        }
    }

}