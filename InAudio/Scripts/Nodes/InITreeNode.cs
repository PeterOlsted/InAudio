using System.Collections.Generic;
using UnityEngine;

namespace InAudioSystem.Internal
{
    [System.Serializable]
    public class EditorSettings
    {
        public bool IsFoldedOut;
        public bool IsFiltered;
    }

    public interface InITreeNode<T> where T : Object, InITreeNode<T>
    {
        T _getParent { get; set; }

        List<T> _getChildren { get; }

        bool IsRoot { get; }

        string GetName { get; }

        int _ID { get; set; }

        MonoBehaviour[] GetAuxData();

#if UNITY_EDITOR
        EditorSettings EditorSettings { get; set; }
#endif
        bool IsFolder { get; }
    }
}