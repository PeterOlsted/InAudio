using UnityEngine;
using System.Collections.Generic;

namespace InAudioSystem
{
    public class InInteractiveMusic : MonoBehaviour, InITreeNode<InInteractiveMusic>
    {
        public int BeatsPerMinute = 100;
        public int BeatsPerBar = 4;
        public int BeatUnit = 4;

        public Object[] GetAuxData()
        {
            return new Object[] { };
        }

        public string GetName
        {
            get
            {
                return nodeName;
            }
        }
        #if UNITY_EDITOR
        public bool IsFiltered
        {
            get
            {
                return filtered;
            }

            set
            {
                filtered = value;
            }
        }
        #endif

        public bool IsRoot
        {
            get
            {
                return type == InteractiveMusicNodeType.Root;
            }
        }

        public List<InInteractiveMusic> _getChildren
        {
            get
            {
                return children;
            }
        }

        public InInteractiveMusic _getParent
        {
            get
            {
                return parent;
            }

            set
            {
                parent = value;
            }
        }

        public int _ID
        {
            get
            {
                return guid;
            }

            set
            {
                guid = value;
            }
        }

#if UNITY_EDITOR
        public bool IsFoldedOut
        {
            get
            {
                return foldedOut;
            }

            set
            {
                foldedOut = value;
            }
        }
#endif

        public bool IsFolder
        {
            get
            {
                return type == InteractiveMusicNodeType.Folder || IsRoot;
            }
        }


        public InInteractiveMusic parent;

        public int guid;

        public List<InInteractiveMusic> children = new List<InInteractiveMusic>();

        public InteractiveMusicNodeType type;

        public string nodeName;

#if UNITY_EDITOR
        public bool filtered;

        public bool foldedOut;
            #endif 
    }

    public enum InteractiveMusicNodeType
    {
        Root = 0,
        Folder = 1,
        Music = 2,
    }
}