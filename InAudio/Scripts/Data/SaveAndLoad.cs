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
            var go = Resources.Load<GameObject>(location);
            return GetComponents(go);
        }

#if UNITY_EDITOR
        public static GameObject CreatePrefab(GameObject root, string path)
        {
            var go = PrefabUtility.CreatePrefab(path, root);
            Object.DestroyImmediate(root);
            return go;
        }
#endif
    }
}