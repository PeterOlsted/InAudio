using InAudioSystem.Internal;
using UnityEngine;

namespace InAudioSystem
{  
    public class InSettingsNode : InGenericNode<InSettingsNode>, InITreeNode<InSettingsNode>
    {
        public bool IsRoot { get { return true; } }

        public MonoBehaviour[] GetAuxData()
        {
            return new MonoBehaviour[] {};
        }

        public bool IsFolder { get { return true; } }
    }
}
