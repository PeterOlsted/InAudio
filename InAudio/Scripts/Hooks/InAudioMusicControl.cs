using System.Collections.Generic;


namespace InAudioSystem
{
    /// <summary>
    /// Used for InAudioEventHook to control music
    /// </summary>
    [System.Serializable]
    public class InHookMusicControl
    {
        //public MusicState DoForAllMusic = MusicState.Nothing;   
        public List<InEventHookMusicControl> MusicControls = new List<InEventHookMusicControl>();
    }

    [System.Serializable]
    public class InEventHookMusicControl
    {
        public InMusicGroup MusicGroup;
        public MusicState PlaybackControl = MusicState.Playing;
    }
}