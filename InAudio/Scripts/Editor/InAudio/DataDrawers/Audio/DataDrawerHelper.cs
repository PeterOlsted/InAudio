using System.Linq;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace InAudioSystem.InAudioEditor
{
    
public static class DataDrawerHelper
{
    public static void DrawMixer(InAudioNode node)
    {
        var serialized = new SerializedObject(node);
        serialized.Update();
        if (!node.IsRoot)
        {
            bool overrideParent = EditorGUILayout.Toggle("Override Parent Mixer Group", node.OverrideParentMixerGroup);
            if (overrideParent != node.OverrideParentMixerGroup)
            {
                InUndoHelper.RecordObjectFull(new Object[] {node._nodeData, node}, "Override parent mixer group");
                node.OverrideParentMixerGroup = overrideParent;
            }
            if (!node.OverrideParentMixerGroup)
                GUI.enabled = false;
        }
        
        EditorGUILayout.BeginHorizontal();

        if (node.IsRoot)
        {
            EditorGUILayout.PropertyField(serialized.FindProperty("MixerGroup"), new GUIContent("Mixer Group"));
        }
        else if (node.OverrideParentMixerGroup)
        {
            EditorGUILayout.PropertyField(serialized.FindProperty("MixerGroup"), new GUIContent("Mixer Group"));
        }
        else
        {
            EditorGUILayout.PropertyField(new SerializedObject(node.GetParentMixerGroup()).FindProperty("MixerGroup"), new GUIContent("Parent Mixer Group"));
        }

        GUI.enabled = node.GetMixerGroup() != null;
        if (GUILayout.Button("Find", GUILayout.Width(40)))
        {
            SearchHelper.SearchFor(node.MixerGroup);
        }
        EditorGUILayout.EndHorizontal();
        serialized.ApplyModifiedProperties();
            GUI.enabled = true;
    }

    public static void DrawMixer(InMusicNode node, SerializedProperty prop)
    {
        if (!node.IsRoot)
        {
            bool overrideParent = EditorGUILayout.Toggle("Override Parent Mixer", node._overrideParentMixerGroup);
            if (overrideParent != node._overrideParentMixerGroup)
            {
                InUndoHelper.RecordObjectFull(node, "Override parent mixer group");
                node._overrideParentMixerGroup = overrideParent;
            }
            if (!node._overrideParentMixerGroup)
                GUI.enabled = false;
        }
        EditorGUILayout.BeginHorizontal();

        if (node._overrideParentMixerGroup)
            EditorGUILayout.PropertyField(prop, new GUIContent("Mixer Group"));
        else
        {
            if (node._parent != null)
            {
                var parentProp = new SerializedObject(node.GetParentMixing());
                parentProp.Update();
                EditorGUILayout.PropertyField(parentProp.FindProperty("_mixerGroup"),
                    new GUIContent("Parent Mixer Group"));
                parentProp.ApplyModifiedProperties();
            }
            else
            {
                var parentProp = new SerializedObject(node);
                parentProp.Update();
                EditorGUILayout.PropertyField(parentProp.FindProperty("_mixerGroup"), new GUIContent("Mixer Group"));
                parentProp.ApplyModifiedProperties();
            }
        }

        GUI.enabled = node.GetParentMixing() != null;
        if (GUILayout.Button("Find", GUILayout.Width(40)))
        {
            SearchHelper.SearchFor(node._mixerGroup);
        }
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;
    }

       
        public static void DrawVolume(Object undoObj, ref float refMinVolume, ref float refMaxVolume, ref bool refRandomVolume)
        {

            float minVolume = refMinVolume;
            float maxVolume = refMaxVolume;
            bool randomVolume = refRandomVolume;

            InUndoHelper.GUIUndo(undoObj, "Random Volume", ref randomVolume, () => EditorGUILayout.Toggle("Random Volume", randomVolume));

            if (!randomVolume)
            {
                InUndoHelper.GUIUndo(undoObj, "Volume", () => EditorGUILayout.Slider("Volume", minVolume, 0, 1), v =>
                {
                    minVolume = v;
                    if (minVolume > maxVolume)
                    {
                        maxVolume = Mathf.Clamp01(minVolume + 0.1f);
                    }
                });
            }
            else
            {
                InUndoHelper.GUIUndo(undoObj, "Random Volume", ref minVolume, ref maxVolume, (out float newMinVolume, out float newMaxVolume) =>
                {
                    EditorGUILayout.MinMaxSlider(new GUIContent("Volume"), ref minVolume, ref maxVolume, 0, 1);
                    newMinVolume = Mathf.Clamp(EditorGUILayout.FloatField("Min volume", minVolume), 0, maxVolume);
                    newMaxVolume = Mathf.Clamp(EditorGUILayout.FloatField("Max volume", maxVolume), minVolume, 1);
                });
            }
            refMinVolume = minVolume;
            refMaxVolume = maxVolume;
            refRandomVolume = randomVolume;
        }
    }
}