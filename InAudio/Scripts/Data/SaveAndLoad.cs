using System;
using InAudioSystem.Internal;
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
        private const int levelSize = 3;
        public static GameObject CreatePrefab<T>(ref T node, Func<GameObject, int, T> create, string path) where T : MonoBehaviour, InITreeNode<T>
        {
            GameObject go = new GameObject();
            node = create(go, levelSize);
            go = CreatePrefab(node.gameObject, path);
            go.GetComponent<T>().EditorSettings.IsFoldedOut = true;
            return go;
        }

        public static GameObject CreatePrefab(GameObject root, string path)
        {
            var go = PrefabUtility.CreatePrefab(path, root);
            Object.DestroyImmediate(root);
            return go;
        }
#endif
    }
}