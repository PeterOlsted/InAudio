using UnityEngine;

public class PlayMusicExample : MonoBehaviour
{
    public AudioParameters Parameters;
    public Vector3 MyV;
    public A a = new A();

    public InMusicGroup musicExample;

    void OnEnable()
    {
        InAudio.Music.Play(musicExample);
        InAudio.Music.PlayAt(musicExample, AudioSettings.dspTime + 2.0f); //Play two seconds in the future
        musicExample.Volume = 0.5f;
        musicExample.Pitch = 2.0f;
        musicExample.Solo = true;
        musicExample.Mute = false;
    }

    void OnDisable()
    {
        InAudio.Music.Stop(musicExample);
        InAudio.Music.FadeAndStop(musicExample, 1.5f); //Fade out in 1.5 and then stop
    }
}

[System.Serializable]
public class A
{
    public float a;
}