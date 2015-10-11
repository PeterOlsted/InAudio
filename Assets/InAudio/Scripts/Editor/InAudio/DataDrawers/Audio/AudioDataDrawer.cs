using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{
public static class AudioDataDrawer
{
    
    public static void Draw(InAudioNode node)
    {
        node.ScrollPosition = GUILayout.BeginScrollView(node.ScrollPosition);

        InUndoHelper.GUIUndo(node, "Name Change", ref node.Name, () => 
            EditorGUILayout.TextField("Name", node.Name));

        Rect area = GUILayoutUtility.GetLastRect();
        
        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        InAudioData audioData = node._nodeData as InAudioData;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();

        var clip = (AudioClip)EditorGUILayout.ObjectField(audioData._clip, typeof(AudioClip), false);
  
        Rect buttonArea = area;
        if (Application.isPlaying)
        {
            buttonArea.x += buttonArea.width - 100;
            buttonArea.width = 70;
            GUI.enabled = false;
            EditorGUI.LabelField(buttonArea, "Is Loaded");
            buttonArea.x += 70;
            buttonArea.width = 10;
            EditorGUI.Toggle(buttonArea, audioData.IsLoaded);
            GUI.enabled = true;
        }

        AudioSource source = InAudioInstanceFinder.Instance.GetComponent<AudioSource>();
        AudioPreview(node, source, audioData);


        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        if (clip != audioData._clip) //Assign new clip
        {
            InUndoHelper.RecordObjectFull(audioData, "Changed " + node.Name + " Clip");
            audioData._clip = clip;            
            EditorUtility.SetDirty(node._nodeData.gameObject);
        }

        EditorGUILayout.EndHorizontal();

        if (clip != null)
        {
            DrawImportSettings(clip);
        }

        NodeTypeDataDrawer.Draw(node);

        GUILayout.EndScrollView();
    }

    private static void DrawImportSettings(AudioClip clip)
    {
        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        var assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(clip)) as AudioImporter;
        bool preload = EditorGUILayout.Toggle("Is preloaded", clip.preloadAudioData);
        bool loadInBackground = EditorGUILayout.Toggle("Load in background", clip.loadInBackground);
        bool mono = EditorGUILayout.Toggle("Mono", assetImporter.forceToMono);
        if (preload != clip.preloadAudioData || loadInBackground != clip.loadInBackground ||
            mono != assetImporter.forceToMono)
        {
            if (EditorUtility.DisplayDialog("Apply setting?", "Change import setting?", "Ok", "Cancel"))
            {
                assetImporter.preloadAudioData = preload;
                assetImporter.loadInBackground = loadInBackground;
                assetImporter.forceToMono = mono;
                assetImporter.SaveAndReimport();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private static void AudioPreview(InAudioNode node, AudioSource source, InAudioData audioData)
    {
        if (source != null && !source.isPlaying)
        {
            source.clip = null;
            source.outputAudioMixerGroup = null;
        }

        if (GUILayout.Button("Preview", GUILayout.Width(60)))
        {
            //AudioSource source = InAudioInstanceFinder.Instance.GetComponent<AudioSource>();
            var root = TreeWalker.FindParentBeforeFolder(node);
            if (source != null)
            {
                source.SetLoudness(RuntimeHelper.CalcVolume(root, node));
                source.pitch = RuntimeHelper.CalcPitch(root, node);
                source.clip = audioData._clip;
                source.outputAudioMixerGroup = node.GetMixerGroup();
                source.Play();
            }
            else
                Debug.LogError(
                    "InAudio: Could not find preview audio source in the InAudio Manager.\nTry to restore the manager from the prefab");
        }

        if (GUILayout.Button("Raw", GUILayout.Width(45)))
        {
            //AudioSource source = InAudioInstanceFinder.Instance.GetComponent<AudioSource>();
            if (source != null)
            {
                source.clip = audioData._clip;
                source.volume = 1.0f;
                source.outputAudioMixerGroup = null;
                source.pitch = 1.0f;
                source.Play();
            }
            else
                Debug.LogError(
                    "InAudio: Could not find preview audio source in the InAudio Manager.\nTry to restore the manager from the prefab");
        }

        if (GUILayout.Button("Stop", GUILayout.Width(45)))
        {
            //AudioSource source = InAudioInstanceFinder.Instance.GetComponent<AudioSource>();
            if (source != null)
            {
                source.Stop();
                source.clip = null;
                source.outputAudioMixerGroup = null;
            }
            else
                Debug.LogError(
                    "InAudio: Could not find preview audio source in the InAudio Manager.\nTry to restore the manager from the prefab");
        }
    }
}
}