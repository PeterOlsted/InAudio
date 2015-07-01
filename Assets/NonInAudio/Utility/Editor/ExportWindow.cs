using UnityEngine;
using UnityEditor;
using InAudioSystem.ExtensionMethods;

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

        if(GUILayout.Button("Export InAudio", GUILayout.Width(200)))
        {
            string[] newPaths = paths.Convert(path => "Assets/" + path);
            AssetDatabase.ExportPackage(newPaths, name, ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
        }
    }

    [MenuItem("Window/InAudio/Export Window", priority =100)]
    private static void Launch()
    {
        GetWindow<ExportWindow>().Show();

    }
}
