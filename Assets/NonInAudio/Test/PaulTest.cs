using UnityEngine;

public class PaulTest : MonoBehaviour
{
    public InMusicGroup musicGroup;
    public float future = 0f;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            PlayMusicGroup(musicGroup);
        }
    }

    public void PlayMusicGroup(InMusicGroup musicGroupName)
    {
        double nextEventTime = AudioSettings.dspTime + future;
        Debug.Log("Status - Playing: " + InAudio.Music.IsPlaying(musicGroup) + " Stopped: " + InAudio.Music.IsStopped(musicGroup));
        if (InAudio.Music.IsStopped(musicGroup))
        {
            //InAudio.Music.Play(musicGroup);
            InAudio.Music.PlayAt(musicGroup, nextEventTime);
        }

    }
}
