using System;
using UnityEngine;

/// <summary>
/// Used to set initial parameters when playing an audio node. 
/// Changing these settings after calling Play() does not affect the already playing audio.
/// </summary>
[System.Serializable]
public class AudioParameters
{
    public float Volume
    {
        get { return volume; }
        set { volume = Mathf.Clamp01(value); }
    }
    public float Pitch
    {
        get { return pitch; }
        set { pitch = Mathf.Clamp(value, 0f, 3f); }
    }
    public float StereoPan
    {
        get { return stereoPan; }
        set { stereoPan = Mathf.Clamp(value, -1f, 1f); }
    }
    public float SpatialBlend
    {
        get { return spatialBlend; }
        set { spatialBlend = Mathf.Clamp01(value); }
    }


    [SerializeField]
    private float volume;
    [SerializeField]
    private float pitch;
    [SerializeField]
    private float stereoPan;
    [SerializeField]
    private float spatialBlend;

    public AudioParameters(float volume, float pitch = 1.0f, float stereoPan = 0.0f, float spatialBlend = 1.0f)
    {
        this.volume = Mathf.Clamp01(volume);
        this.pitch = Mathf.Clamp(pitch, 0f, 3f);
        this.stereoPan = Mathf.Clamp(stereoPan, -1f, 1f);
        this.spatialBlend = Mathf.Clamp01(spatialBlend);
    }

    public void Reset()
    {
        volume = 1.0f;
        pitch = 1.0f;
        stereoPan = 0.0f;
        spatialBlend = 1.0f;
    }

    public void CopyFrom(AudioParameters parameters)
    {
        volume = parameters.volume;
        pitch = parameters.pitch;
        stereoPan = parameters.stereoPan;
        spatialBlend = parameters.spatialBlend;
    }

    public AudioParameters(AudioParameters parameters)
    {
        volume = parameters.volume;
        pitch = parameters.pitch;
        stereoPan = parameters.stereoPan;
        spatialBlend = parameters.spatialBlend;
    }

    public AudioParameters()
    {
        volume = 1.0f;
        pitch = 1.0f;
        stereoPan = 0.0f;
        spatialBlend = 1.0f;
    }
}