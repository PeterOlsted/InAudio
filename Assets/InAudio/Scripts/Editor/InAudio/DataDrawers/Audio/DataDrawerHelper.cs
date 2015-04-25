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
                UndoHelper.RecordObjectFull(new Object[] {node._nodeData, node}, "Override parent mixer group");
                node.OverrideParentMixerGroup = overrideParent;
            }
            if (!node.OverrideParentMixerGroup)
                GUI.enabled = false;
        }
        EditorGUILayout.BeginHorizontal();

        if (node.OverrideParentMixerGroup)
            EditorGUILayout.PropertyField(serialized.FindProperty("MixerGroup"), new GUIContent("Mixer Group"));
        else
            EditorGUILayout.PropertyField(new SerializedObject(node.GetParentMixerGroup()).FindProperty("MixerGroup"), new GUIContent("Parent Mixer Group"));

        GUI.enabled = node.GetMixerGroup() != null;
        if (GUILayout.Button("Find", GUILayout.Width(40)))
        {
            SearchHelper.SearchFor(node.MixerGroup);
        }
        EditorGUILayout.EndHorizontal();
        serialized.ApplyModifiedProperties();
    }

    public static void DrawMixer(InMusicNode node, SerializedProperty prop)
    {
        if (!node.IsRoot)
        {
            bool overrideParent = EditorGUILayout.Toggle("Override Parent Mixer", node._overrideParentMixerGroup);
            if (overrideParent != node._overrideParentMixerGroup)
            {
                UndoHelper.RecordObjectFull(node, "Override parent mixer group");
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
        }

        GUI.enabled = node.GetParentMixing() != null;
        if (GUILayout.Button("Find", GUILayout.Width(40)))
        {
            SearchHelper.SearchFor(node._mixerGroup);
        }
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;
    }
}
}