using System.Collections.Generic;
using InAudioSystem;
using UnityEngine;
using UnityEngine.Serialization;


public class InMusicGroup : InMusicNode
{
    #region Playback info
    public bool Playing
    {
        get { return PlayingInfo.State == MusicState.Playing; }
    }

    public bool Paused
    {
        get { return PlayingInfo.State == MusicState.Paused; }
    }

    public bool Stopped
    {
        get { return PlayingInfo.State == MusicState.Stopped || PlayingInfo.State == MusicState.Nothing; }
    }

    #endregion
        
    #region Properties

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

    public bool AffectedByMute
    {
        get { return PlayingInfo.AffectedByMute; }
        //set automatically depending if the parent is muted. See InAudio.Update()
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
#endregion        

    #region Runtime
    [System.NonSerialized]
    //This class may change at any time, you should not dependent on it for your programming.
    public PlayingMusicInfo PlayingInfo = new PlayingMusicInfo();

    public float runtimeVolume; //Only used during runtime. Should be able to be safely modified, but using InAudio.Music is recommended
    public float runtimePitch;
    public bool runtimeSolo;
    public bool runtimeMute;

    #endregion

    #region Private data
    //IMPORTANT!
    //Not recommended to change these directly.
    //Changes done while in editor, even during runtime, will permanently change the values in the prefabs.

    public bool _loop = true;

    public bool _autoStart = false;
    public float _pitch = 1.0f;

    public bool _solo;
    public bool _mute;

    [FormerlySerializedAs("_editorClips")]
    public List<AudioClip> _clips = new List<AudioClip>();

    public List<AudioClip> _Clips
    {
        #if UNITY_EDITOR
            get
            {
                return _clips; 
            }
            set { _clips = value; }
        #else
            get
            {
                return _clips;
            }
            //set
            //{
            //    _clips = value;
            //}
        #endif
    }
    #endregion
}
