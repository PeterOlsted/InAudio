//using UnityEngine;

//public class InPlaylistController : MonoBehaviour
//{
//    public InPlaylist CurrentPlaylist;
//    public InPlaylist PreviousPlaylist;

//    public bool PlayOnEnable
//    {
//        get { return playOnEnable; }
//    }

//    public bool StopOnDisable
//    {
//        get { return stopOnDisable; }
//    }

//    public void CrossfadePlaylist(InPlaylist newPlaylist, float time = 1)
//    {
//        if (time < 0.0001)
//        {
//            Stop();
//            Play(newPlaylist);
//            PreviousPlaylist = CurrentPlaylist;
//            CurrentPlaylist = newPlaylist;
//        }
//        else
//        {
//            Stop();
//            Play(newPlaylist);
//            PreviousPlaylist = CurrentPlaylist;
//            CurrentPlaylist = newPlaylist;
//        }
//    }

//    public bool Play(InPlaylist playlist)
//    {
//        if (playlist == null)
//        {
//            return false;
//        }
//        CurrentPlaylist = playlist;
//        playlist.Play();
//        return true;
//    }

//    public void Pause(InPlaylist playlist)
//    {
//        if (playlist != null)
//        {
//            //playlist.
//        }
//    }

//    public void Stop(InPlaylist playlist)
//    {
//        if (playlist != null && playlist.PlayingCurrently != null)
//        {
//            InAudio.Music.Stop(playlist.PlayingCurrently);
//        }
//    }

//    public void Play()
//    {
//        Play(CurrentPlaylist);
//    }

//    public void Pause()
//    {
//        Pause(CurrentPlaylist);
//        Pause(PreviousPlaylist);
//    }

//    public void Stop()
//    {
//        Stop(CurrentPlaylist);
//        Stop(PreviousPlaylist);
//    }
  

//    [SerializeField]
//    private bool playOnEnable = true;
//    [SerializeField]
//    private bool stopOnDisable = true;

//    private void OnEnable()
//    {
//        if (PlayOnEnable)
//        {
//            Play(CurrentPlaylist);
//        }
//    }

//    private void OnDisable()
//    {
//        if (StopOnDisable)
//        {
//            Stop(CurrentPlaylist);
//            Stop(PreviousPlaylist);
//        }
//    }
//}