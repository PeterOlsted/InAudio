using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace InAudioSystem
{

    public enum AudioBankTypes
    {
        Folder = 0,
        Bank = 1,
    }


    public class InAudioBankLink : MonoBehaviour, InITreeNode<InAudioBankLink>
    {
        [FormerlySerializedAs("GUID")]
        public int _guid;

        [FormerlySerializedAs("_type")]
        public AudioBankTypes _type;

        [FormerlySerializedAs("Name")]
        public string _name;

        [FormerlySerializedAs("Parent")]
        public InAudioBankLink _parent;

        [FormerlySerializedAs("Children")]
        public List<InAudioBankLink> _children = new List<InAudioBankLink>();

        [FormerlySerializedAs("AutoLoad")]
        public bool _autoLoad = false;

        public List<BankData> _bankData = new List<BankData>();

        public Object[] GetAuxData()
        {
            return new Object[] {};
        }

#if UNITY_EDITOR
        public bool FoldedOut;

        public bool Filtered = false;
#endif

        [System.NonSerialized] private bool isLoaded;

        public bool IsLoaded
        {
            get { return isLoaded; }
            set { isLoaded = value; }
        }

        public InAudioBankLink _getParent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public List<InAudioBankLink> _getChildren
        {
            get { return _children; }
        }

        public string GetName
        {
            get { return _name; }
        }

        public bool IsRoot
        {
            get { return _parent == null; }
        }

        public int _ID
        {
            get { return _guid; }
            set { _guid = value; }
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

        public bool IsFolder
        {
            get { return _type == AudioBankTypes.Folder; }
        }
    }

}