using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    [CustomEditor(typeof(AudioSourcePlayVisualizer))]
    public class AudioSourcePlayVisualizerDrawer : Editor
    {
        AudioSourcePlayVisualizer Target
        {
            get { return target as AudioSourcePlayVisualizer; }
        }

        private AudioSource[] AudioSources;

        public override void OnInspectorGUI()
        {
            AudioSources = Target.GetComponents<AudioSource>();

            for (int i = 0; i < AudioSources.Length; i++)
            {
                var source = AudioSources[i];
                var clip = source.clip;
                if (clip != null)
                {
                    EditorGUILayout.LabelField(clip.name);
                    EditorGUILayout.Slider((float) source.ExactPosition(), 0, clip.ExactLength());
                }
            }
            
            if(Application.isPlaying)
                EditorUtility.SetDirty(target);

        }
    }

}