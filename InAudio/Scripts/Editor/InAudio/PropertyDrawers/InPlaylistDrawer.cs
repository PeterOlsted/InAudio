using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof (InPlaylist))]
    public class InPlaylistDrawer : Editor
    {
        private InPlaylist Playlist
        {
            get { return serializedObject.targetObject as InPlaylist; }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Play")) 
                {
                    Playlist.Play();
                }
                if (GUILayout.Button("Pause"))
                {
                    Playlist.Pause();
                }
                if (GUILayout.Button("Stop"))
                {
                    Playlist.Stop();
                }
                if (GUILayout.Button("Stop Track"))
                {
                    Playlist.StopCurrentlyPlaying();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("musicParameters"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("musicGroups"), true);

            var shuffle = serializedObject.FindProperty("shuffle");
            bool currentState = shuffle.boolValue;

            EditorGUILayout.PropertyField(shuffle, true);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("crossfade"));

            if(serializedObject.FindProperty("crossfade").boolValue)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("crossfadeTime"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("delayBetweenTracks"));

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playOnEnable"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("stopOnDisable"));

            if (Application.isPlaying)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("playlist"), true);
                GUI.enabled = true;
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                if (shuffle.boolValue != currentState)
                {
                    (target as InPlaylist).GeneratePlaylist();
                }
            }
        }
    }
}
