using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InAudioSystem
{
    public static class SaveAndLoad
    {
        private static Component[] GetComponents(GameObject go)
        {
            if (go != null)
            {
                return go.GetComponentsInChildren(typeof(MonoBehaviour), true);
            }
            return null;
        }

        public static Component[] LoadManagerData(string location)
        {
            return GetComponents(Resources.Load(location) as GameObject);
        }

#if UNITY_EDITOR
        public static void CreateDataPrefabs(GameObject AudioRoot, GameObject MusicRoot, GameObject EventRoot)
        {
            CreateMusicRootPrefab(MusicRoot);
            CreateAudioNodeRootPrefab(AudioRoot);
            CreateAudioEventRootPrefab(EventRoot);
        }

        public static void CreateAudioNodeRootPrefab(GameObject root)
        {
            PrefabUtility.CreatePrefab(FolderSettings.AudioSaveDataPath, root);
            Object.DestroyImmediate(root);
        }
        public static void CreateAudioEventRootPrefab(GameObject root)
        {
            PrefabUtility.CreatePrefab(FolderSettings.EventSaveDataPath, root);
            Object.DestroyImmediate(root);
        }
        public static void CreateMusicRootPrefab(GameObject root)
        {
            PrefabUtility.CreatePrefab(FolderSettings.MusicSaveDataPath, root);
            Object.DestroyImmediate(root);
        }
#endif
    }
}