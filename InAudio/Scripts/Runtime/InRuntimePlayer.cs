using System;
using InAudioSystem.Runtime;
using UnityEngine;

//<summary>
 //The class that actually plays the audio
 //</summary>
[RequireComponent(typeof(AudioSource))]
public class InRuntimePlayer : MonoBehaviour
{
    [NonSerialized] public double StartTime;
    [NonSerialized] public double EndTime;
    [NonSerialized] public DSPTime DSPTime; 
    [NonSerialized] public float OriginalVolume;
    [NonSerialized] public float Rolloff = 1f;

    public AudioSource AudioSource;

    public InAudioNode UsedNode;

    public AudioClip Clip
    {
        get
        {
            if (AudioSource == null)
                return null;
            else
                return AudioSource.clip;
        }
    }

    public void Stop()
    {
        AudioSource.clip = null;
        AudioSource.Stop();
    }
}