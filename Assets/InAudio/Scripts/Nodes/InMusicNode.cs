using System.Collections.Generic;
using InAudioSystem;
using UnityEngine;
using UnityEngine.Audio;

    public class InMusicNode : MonoBehaviour, InITreeNode<InMusicNode>
    {
        public int _guid;

        public string _name = "";

        public MusicNodeType _type;

        public bool _overrideParentMixerGroup;
        public AudioMixerGroup _mixerGroup;

        //MinVolume instead of "Volume" if Min-Max volume is to be added later
        public float _minVolume = 1f;
        public float _minPitch = 1f;

#if UNITY_EDITOR
        public bool Filtered = false;

        public bool FoldedOut;

        public Vector2 ScrollPosition = new Vector2();

#endif

        public Object[] GetAuxData()
        {
            return new Object[] {};
        }

    public List<InMusicNode> _children = new List<InMusicNode>();

        public InMusicNode _parent;

        public InMusicNode _getParent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public List<InMusicNode> _getChildren
        {
            get { return _children; }
        }

        public bool IsRoot
        {
            get { return _type == MusicNodeType.Root; }
        }

        public string GetName
        {
            get { return _name; }
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

        public bool IsRootOrFolder
        {
            get
            {
                if (_type == MusicNodeType.Root || _type == MusicNodeType.Folder)
                    return true;
                return false;
            }
        }

        public bool IsFolder
        {
            get
            {
                if (_type == MusicNodeType.Folder)
                    return true;
                return false;
            }
        }

        public float Volume
        {
            get
            {
                return runtimeVolume;
            }
            set
            {
                runtimeVolume = Mathf.Clamp01(value);
            }
        }

        public float Pitch
        {
            get { return runtimePitch; }
            set
            {
                runtimePitch = Mathf.Clamp(value, 0, 3);
            }
        }

        public bool Solo
        {
            get
            {
                return runtimeSolo;
            }
            set
            {
                runtimeSolo = value;
            }
        }

        public bool Mute
        {
            get
            {
                return runtimeMute;
            }
            set
            {
                runtimeMute = value;
            }
        }

        #region Runtime
        [System.NonSerialized] //This class may change at any time, you should not dependent on it for your programming.
        public PlayingMusicInfo PlayingInfo = new PlayingMusicInfo();

        public float runtimeVolume; //Only used during runtime. Should be able to be safely modified, but using InAudio.Music is recommended
        public float runtimePitch;
        public bool runtimeSolo;
        public bool runtimeMute;

        #endregion

        public bool _solo;
        public bool _mute;


    }
    public enum MusicNodeType
    {
        Root = 0,
        Folder = 1,
        Music = 2,
    }   

