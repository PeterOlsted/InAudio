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

    

    public bool AffectedByMute
    {
        get { return PlayingInfo.AffectedByMute; }
        //set automatically depending if the parent is muted. See InAudio.Update()
    }

    
#endregion        

   

    #region Private data
    //IMPORTANT!
    //Not recommended to change these directly.
    //Changes done while in editor, even during runtime, will permanently change the values in the prefabs.

    public bool _loop = true;

    public bool _autoStart = false;


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
