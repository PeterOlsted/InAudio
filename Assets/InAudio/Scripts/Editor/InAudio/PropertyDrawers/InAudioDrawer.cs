using UnityEditor;
using UnityEngine;
using System.Collections;
using InAudioSystem.Internal;

namespace InAudioSystem
{

    [CustomEditor(typeof (InAudio))]
    public class InAudioDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var inAudio = target as InAudio;

            if (inAudio.GetComponentInChildren<InAudioInstanceFinder>() == null
                || inAudio.GetComponentInChildren<MusicPlayer>() == null
                || inAudio.GetComponentInChildren<InCommonDataManager>() == null)
            {
                //Checks three of the needed objects if this is indeed a valid prefab
                //If you read this code, use the "InAudio Manager" prefab stored under InAudio/Prefabs/InAudio Manager
                EditorGUILayout.HelpBox("You are not using the InAudio Manager prefab.\nPlease use the InAudio prefab placed in \nInAudio/Prefabs/InAudio Manager\ninstead of placing the InAudio script on a random game object.", MessageType.Error);
            }

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