using UnityEngine;
using UnityEngine.Audio;

namespace InAudioSystem.ExtensionMethods
{
    public static class NodeExtensions
    {
        public static void AssignParent<T>(this T node, T newParent) where T : Object, InITreeNode<T>
        {
            if (node != null && newParent != null)
            {
                newParent._getChildren.Add(node);
                node._getParent = newParent;
            }
        }

        public static void MoveToNewParent<T>(this T node, T newParent) where T : Object, InITreeNode<T>
        {
            if (node != null && newParent != null)
            {
                node._getParent._getChildren.Remove(node);
                newParent._getChildren.Add(node);
                node._getParent = newParent;
            }
        }
    }

    public static class AudioNodeExtensions
    {
        public static AudioMixerGroup GetMixerGroup(this InAudioNode node)
        {
            if (node == null)
            {
                return null;
            }
            if (node.OverrideParentMixerGroup || node.IsRoot)
            {
                return node.MixerGroup;
            }
            else
            {
                return GetMixerGroup(node._parent);
            }
        }

        public static InAudioNode GetParentMixerGroup(this InAudioNode node)
        {
            if (node == null)
                return null;
            if (node.OverrideParentMixerGroup || node.IsRoot)
            {
                return node;
            }
            else
                return GetParentMixerGroup(node._parent);
        }
    }

    public static class MusicNodeExtensions
    {
        public static AudioMixerGroup GetUsedMixerGroup(this InMusicNode node)
        {
            if (node == null)
                return null;
            if (node._overrideParentMixerGroup || node.IsRoot)
            {
                return node._mixerGroup;
            }
            else
                return GetUsedMixerGroup(node._parent);
        }

        public static InMusicNode GetParentMixing(this InMusicNode node)
        {
            if (node == null)
                return null;
            if (node._overrideParentMixerGroup || node.IsRoot)
            {
                return node;
            }
            else
                return GetParentMixing(node._parent);
        }
    }
}