using System.Collections.Generic;
using InAudioLeanTween;
using UnityEngine;

namespace InAudioSystem
{

    public class PlayingMusicInfo
    {
        public InMusicGroup Music;
        public MusicState State = MusicState.Stopped;
        public List<AudioSource> Players = new List<AudioSource>();

        public bool Fading = false;
        public LeanTweenType TweenType;
        public float StartVolume;
        public float TargetVolume;
        public float StartTime;
        public float EndTime;
        public MusicState DoAtEnd = MusicState.Nothing;

        public float FinalVolume; //The result of itself, the parent and any mute/solo actions

        public double StartedAtDSPTime;
        public double Progress;

        public bool AffectedByMute; //Is currently muted?

        public bool IsSoloed; //Is currently solo? Meaning all who is not will be silent, if any is solo
        public bool HasSoloedChild;
    }

    public enum MusicState
    {
        Stopped,
        Playing,
        Paused,
        Nothing
    }
}