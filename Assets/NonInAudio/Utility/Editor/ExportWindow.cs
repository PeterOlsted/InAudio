using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;

public class ExportWindow : EditorWindow {

    private string[] paths = new[] {
        "InAudio/Icons",
        "InAudio/Prefabs",
        "InAudio/Scripts",
        "InAudio/Resources/InAudio/Root.png",
        "InAudio/LeanTween",
        };

    private void OnGUI()
    {
        foreach (var path in paths)
        {
            GUILayout.Label(path);
        }
        EditorGUILayout.Separator();
        string name = "InAudio_v" + InAudio.CurrentVersion + ".unitypackage";
        GUI.enabled = false;
        EditorGUILayout.LabelField("Preview", name);
        GUI.enabled = true;

        if(GUILayout.Button("Export InAudio.unitypackage", GUILayout.Width(200)))
        {
            string[] newPaths = paths.Convert(path => "Assets/" + path);
            AssetDatabase.ExportPackage(newPaths, name, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        if (GUILayout.Button("Move InAudio project for Asset Store"))
        {
            string folder = "Assets/TempExport/";
            foreach (var root in InAudioInstanceFinder.DataManager.AllRoots)
            {
                var path = AssetDatabase.GetAssetPath(root.gameObject);

                string from = path;
                string to = folder + root.gameObject.name + ".prefab";
                AssetDatabase.MoveAsset(from, to);
            }
        }
        if (GUILayout.Button("Move InAudio back project for Asset Store"))
        {
            string folder = "Assets/InAudio/Resources/InAudio/";
            foreach (var root in InAudioInstanceFinder.DataManager.AllRoots)
            {
                var path = AssetDatabase.GetAssetPath(root.gameObject);

                string from = path;
                string to = folder + root.gameObject.name + ".prefab";
                AssetDatabase.MoveAsset(from, to);
            }
        }
    }

    [MenuItem("Window/InAudio/Export Window", priority =100)]
    private static void Launch()
    {
        GetWindow<ExportWindow>().Show();

    }
}
