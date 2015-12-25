using System;
using System.Collections.Generic;
using InAudioSystem;
using InAudioSystem.Runtime;
using UnityEngine;
using UnityEngine.Audio;

//namespace InAudioSystem
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class InAudioNode : MonoBehaviour, InITreeNode<InAudioNode>
{
        [FormerlySerializedAs("GUID")]
        public int _guid;

        [FormerlySerializedAs("Type")]
        public AudioNodeType _type;

        [FormerlySerializedAs("NodeData")]
        public InAudioNodeBaseData _nodeData;

        public string Name;

        [FormerlySerializedAs("Parent")]
        public InAudioNode _parent;

        public bool OverrideParentMixerGroup;
        public AudioMixerGroup MixerGroup;

        [FormerlySerializedAs("Children")]
        public List<InAudioNode> _children = new List<InAudioNode>();

        public Object[] GetAuxData()
        {
            return new Object[] {_nodeData};
        }

#if UNITY_EDITOR
    public bool Filtered = false;

        public bool FoldedOut;

        public Vector2 ScrollPosition = new Vector2();

#endif

        //A list of 
        [NonSerialized] public List<InstanceInfo> CurrentInstances = new List<InstanceInfo>(0);


        public InAudioNode _getParent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public List<InAudioNode> _getChildren
        {
            get { return _children; }
        }

        public string GetName
        {
            get { return Name; }
        }

        public bool IsFolder
        {
            get { return _type == AudioNodeType.Folder; }
        }

        public bool IsRoot
        {
            get { return _type == AudioNodeType.Root; }
        }

        public bool IsRootOrFolder
        {
            get { return _type == AudioNodeType.Folder || _type == AudioNodeType.Root; }
        }

        public int _ID
        {
            get { return _guid; }
            set { _guid = value; }
        }

        public bool IsPlayable
        {
            get { return !IsRootOrFolder; }
        }

#if UNITY_EDITOR
        public bool IsFoldedOut
        {
            get { return FoldedOut; }
            set { FoldedOut = value; }
        }

        public bool IsFiltered
        {
            get { return Filtered; }
            set { Filtered = value; }
        }
#endif
}



namespace InAudioSystem
{
    public struct InstanceInfo
    {
        public double Timestamp;
        public InPlayer Player;

        public InstanceInfo(double timestamp, InPlayer player)
        {
            Timestamp = timestamp;
            Player = player;
        }
    }

    

    public enum AudioNodeType
    {
        Root = 0,
        Folder = 1,
        Audio = 2,
        Random = 3,
        Sequence = 4, 
        Voice = 5,
        Multi = 6,
        Track = 7,
    }
}
