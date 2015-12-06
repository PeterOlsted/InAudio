using System.Collections.Generic;
using UnityEngine;

namespace InAudioSystem
{

    public interface InITreeNode<T> where T : UnityEngine.Object, InITreeNode<T>
    {
        T _getParent { get; set; }

        List<T> _getChildren { get; }

        bool IsRoot { get; }

        string GetName { get; }

        int _ID { get; set; }

        Object[] GetAuxData();

#if UNITY_EDITOR
        bool IsFoldedOut { get; set; }

        bool IsFiltered { get; set; }
#endif
        bool IsFolder { get; }
    }
}