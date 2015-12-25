using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace InAudioSystem
{

    public class InAudioEventNode : MonoBehaviour, InITreeNode<InAudioEventNode>
    {
        [FormerlySerializedAs("GUID")]
        public int _guid;

        [FormerlySerializedAs("Type")]
        public EventNodeType _type;

        public string Name;

        [FormerlySerializedAs("Parent")]
        public InAudioEventNode _parent;

        public float Delay;

        [FormerlySerializedAs("Children")]
        public List<InAudioEventNode> _children = new List<InAudioEventNode>();

        [FormerlySerializedAs("ActionList")]
        public List<AudioEventAction> _actionList = new List<AudioEventAction>();

        public Object[] GetAuxData()
        {
            return _actionList.ToArray();
        }


        public void AssignParent(InAudioEventNode node)
        {
            node._children.Add(this);
            _parent = node;
        }

        public InAudioEventNode _getParent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public List<InAudioEventNode> _getChildren
        {
            get { return _children; }
        }

        #if UNITY_EDITOR
        public bool FoldedOut;

        public bool IsFoldedOut
        {
            get { return FoldedOut; }
            set { FoldedOut = value; }
        }

        public bool Filtered;

        public bool IsFiltered
        {
            get { return Filtered; }
            set { Filtered = value; }
        }
        #endif

        public bool IsFolder
        {
            get { return _type == EventNodeType.Folder; }
        }

        public bool PlacedExternaly;

        public string GetName
        {
            get { return Name; }
        }

        public bool IsRoot
        {
            get { return _type == EventNodeType.Root; }
        }

        public bool IsRootOrFolder
        {
            get { return _type == EventNodeType.Root || _type == EventNodeType.Folder; }
        }

        public int _ID
        {
            get { return _guid; }
            set { _guid = value; }
        }
    }

    public enum EventNodeType
    {
        Root,
        Folder,
        EventGroup,
        Event
    }
    

}