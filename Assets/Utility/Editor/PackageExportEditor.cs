using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(PackageExporter))]
public class PackageExportEditor : Editor {

    private PackageExporter PackageExporter
    {
        get { return target as PackageExporter; }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Build InAudio"))
        {
            PackageExporter.Build(PackageExporter.InAudio);
        }
        if (GUILayout.Button("Build Sample"))
        {
            PackageExporter.Build(PackageExporter.Sample);
        }
    }
}
