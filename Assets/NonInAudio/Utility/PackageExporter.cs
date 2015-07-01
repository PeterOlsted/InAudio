using InAudioSystem.ExtensionMethods;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PackageExporter : MonoBehaviour
{
#if UNITY_EDITOR
    public Package InAudio;
    public Package Sample;

    public void Build(Package package)
    {
        Debug.Log("Building " +package.Name);
        string[] paths = package.Paths.Convert(path => "Assets/" + path);
        AssetDatabase.ExportPackage(paths, package.Name + ".unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse);
    }

    [System.Serializable]
    public class Package
    {
        public string Name;
        public string[] Paths;
    }

#endif
}
