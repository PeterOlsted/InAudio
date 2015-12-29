using System.Collections.Generic;
using System.Linq;
using InAudioSystem;
using InAudioSystem.ExtensionMethods;
using UnityEngine;

[AddComponentMenu("InAudio/Music/Playlist")]
public class InPlaylist : MonoBehaviour
{
    public List<InMusicGroup> MusicGroups
    {
        get { return musicGroups; }
        set { musicGroups = value; }
    }
    
    public bool Shuffle
    {
        get { return shuffle; }
        set
        {
            if (shuffle != value)
            {
                shuffle = value;
                GeneratePlaylist();
            }
        }
    }

    public bool Crossfade
    {
        get { return crossfade; }
        set { crossfade = value; }
    }

    public bool PlayOnEnable
    {
        get { return playOnEnable; }
        set { playOnEnable = value; }
    }

    public bool StopOnDisable
    {
        get { return stopOnDisable; }
        set { stopOnDisable = value; }
    }

    public float TrackCrossfadeTime
    {
        get { return crossfadeTime; }
        set { crossfadeTime = Mathf.Max(value, 0.0f); }
    }

    public InMusicGroup PlayingCurrently
    {
        get { return playingCurrently; }
    }

    public InMusicGroup PlayingPreviously
    {
        get { return playingPreviously; }
    }

    public bool IsPlaying
    {
        get { return musicState == MusicState.Playing; }
    }

    public bool IsPaused
    {
        get { return musicState == MusicState.Paused; }
    }

    public bool IsStopped
    {
        get { return musicState == MusicState.Stopped || musicState == MusicState.Nothing; }
    }

    public bool IsTransitioningToNextSong
    {
        get { return transitionToNextSong; }
    }


    public float DelayBetweenTracks
    {
        get { return delayBetweenTracks; }
    }

    public InMusicGroup CurrentMusic
    {
        get { return playingCurrently; }
    }

    public InMusicGroup PreviousMusic
    {
        get { return playingPreviously; }
    }

    public MusicState MusicState { get { return musicState; } }

    public bool Play()
    {
        if (musicGroups.Count == 0)
        {
            return false;
        }

        if (musicState == MusicState.Paused && playingCurrently != null)
        {
            musicState = MusicState.Playing;
            InAudio.Music.Play(playingCurrently);
            return true;
        }
        else
        {
            if (IsPlaying)
            {
                Stop();
            }

            var music = GetNextTrack();
            
            musicState = MusicState.Playing;

            SetCurrentlyPlaying(music);
            if (music == null)
            {
                return false;
            }
            InAudio.Music.Play(music);
            return true;
        }
    }

    public void Pause()
    {
        musicState = MusicState.Paused;
        if (PlayingCurrently != null)
            InAudio.Music.Pause(PlayingCurrently);
        if(PlayingPreviously != null)
            InAudio.Music.Pause(PlayingPreviously);
    }

    /// <summary>
    /// Stops the current track without resetting the playlist
    /// </summary>
    public void StopCurrentlyPlaying()
    {
        musicState = MusicState.Stopped;
        if (PlayingCurrently != null)
            InAudio.Music.Stop(PlayingCurrently);
        if (PlayingPreviously != null)
            InAudio.Music.Stop(PlayingPreviously);
    }

    public void Stop()
    {
        musicState = MusicState.Stopped;
        if (PlayingCurrently != null)
            InAudio.Music.Stop(PlayingCurrently);
        if (PlayingPreviously != null)
            InAudio.Music.Stop(PlayingPreviously);
        playingCurrently = null;
        playingPreviously = null;
    }

    public InMusicGroup GetNextTrack()
    {
        if (playingCurrently == null || playlist.Count < 2)
        {
            return musicGroups.FirstOrDefault();
        }
        else
        {
            int i = playlist.FindIndex(playingCurrently);
            return playlist[(i + 1) % playlist.Count];
        }
    }

    public void GeneratePlaylist()
    {
        musicGroups.CopyTo(playlist);
        if (Shuffle)
        {
            ShuffleList(playlist);
        }
    }

    private void SetCurrentlyPlaying(InMusicGroup group)
    {
        playingCurrently = group;
    }

    [SerializeField]
    private MusicParameters musicParameters = new MusicParameters();

    [SerializeField]
    private List<InMusicGroup> musicGroups  = new List<InMusicGroup>();

    [SerializeField]
    private MusicState musicState;

    [SerializeField]
    private bool shuffle;

    [SerializeField]
    private bool crossfade = false;

    [SerializeField]
    [Range(0, 60)]
    private float crossfadeTime = 3;

    [SerializeField]
    [Range(0, 60)]
    private float delayBetweenTracks = 0;

    [SerializeField] 
    private List<InMusicGroup> playlist;

    [SerializeField]
    private bool transitionToNextSong = false;

    [SerializeField]
    private bool playOnEnable = true;
    [SerializeField]
    private bool stopOnDisable = true;

    private InMusicGroup playingCurrently;
    private InMusicGroup playingPreviously;

    private const float prebookAudioTime = 5f;

    private void Update()
    {
        if (playingCurrently != null && musicState == MusicState.Playing)
        {
            double remainingTime = InAudio.Music.GetRemainingTime(playingCurrently);
            var next = GetNextTrack();
            
            if (playingPreviously != null && playingPreviously.Stopped)
            {
                transitionToNextSong = false;
            }
            if (!transitionToNextSong && next != playingCurrently)
            {
                if (crossfade)
                {
                    if (remainingTime <= crossfadeTime)
                    {
                        float pitch = InAudio.Music.GetHiearchyPitch(playingCurrently);
                        float nextPitch = InAudio.Music.GetHiearchyPitch(next);

                        transitionToNextSong = true;
                        InAudio.Music.FadeAndStop(playingCurrently, (float) remainingTime/pitch);
                        InAudio.Music.PlayWithFadeInAt(next, musicParameters.Volume, (float) remainingTime/ nextPitch, AudioSettings.dspTime + delayBetweenTracks);
                        playingPreviously = playingCurrently;
                        playingCurrently = next;
                    }
                }
                else if (remainingTime/playingCurrently.Pitch <= prebookAudioTime)
                {
                    float pitch = InAudio.Music.GetHiearchyPitch(playingCurrently);
                    float nextPitch = InAudio.Music.GetHiearchyPitch(next);

                    transitionToNextSong = true;
                    InAudio.Music.StopAt(playingCurrently, AudioSettings.dspTime + remainingTime/pitch);
                    InAudio.Music.PlayWithFadeInAt(next, (float)remainingTime / nextPitch, AudioSettings.dspTime + remainingTime/nextPitch + delayBetweenTracks);
                    playingPreviously = playingCurrently;
                    playingCurrently = next;
                    playingCurrently.Volume = musicParameters.Volume;
                }
            }
        }
    }

    private void OnEnable()
    {
        if (PlayOnEnable)
        {
            Play();
        }
    }

    private void OnDisable()
    {
        if (StopOnDisable)
        {
            Stop();
        }
    }

    private void Awake()
    {
        GeneratePlaylist();
    }

    //http://stackoverflow.com/a/1262619
    private void ShuffleList<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}