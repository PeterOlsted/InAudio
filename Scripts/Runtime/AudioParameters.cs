using System;
using UnityEngine;
using UnityEngine.Audio;

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

    public AudioMixerGroup AudioMixer
    {
        get { return audioMixer; }
        set { audioMixer = value; }
    }

    /// <summary>
    /// Sets if the audiomixer settings should be used. If false, the audio mixer will be overriden when playing
    /// </summary>
    public bool SetMixer
    {
        get { return setMixer; }
        set { setMixer = value; }
    }

    public AudioParameters()
    {
        volume = 1.0f;
        pitch = 1.0f;
        stereoPan = 0.0f;
        spatialBlend = 1.0f;
        audioMixer = null;
        setMixer = false;
    }

    public AudioParameters(AudioParameters parameters)
    {
        volume = parameters.volume;
        pitch = parameters.pitch;
        stereoPan = parameters.stereoPan;
        spatialBlend = parameters.spatialBlend;
        audioMixer = parameters.audioMixer;
        setMixer = parameters.setMixer;
    }

    public AudioParameters(float volume, float pitch = 1.0f, float stereoPan = 0.0f, float spatialBlend = 1.0f)
    {
        this.volume = Mathf.Clamp01(volume);
        this.pitch = Mathf.Clamp(pitch, 0f, 3f);
        this.stereoPan = Mathf.Clamp(stereoPan, -1f, 1f);
        this.spatialBlend = Mathf.Clamp01(spatialBlend);
        setMixer = false;
    }

    public AudioParameters(float volume, float pitch = 1.0f, float stereoPan = 0.0f, float spatialBlend = 1.0f, AudioMixerGroup mixer = null)
    {
        this.volume = Mathf.Clamp01(volume);
        this.pitch = Mathf.Clamp(pitch, 0f, 3f);
        this.stereoPan = Mathf.Clamp(stereoPan, -1f, 1f);
        this.spatialBlend = Mathf.Clamp01(spatialBlend);
        this.audioMixer = mixer;
        setMixer = true;

    }

    /// <summary>
    /// Reset all parameters to its default values
    /// </summary>
    public void Reset()
    {
        volume = 1.0f;
        pitch = 1.0f;
        stereoPan = 0.0f;
        spatialBlend = 1.0f;
        audioMixer = null;
        setMixer = false;
    }

    /// <summary>
    /// Copy values from another instance of this class
    /// </summary>
    /// <param name="parameters"></param>
    public void CopyFrom(AudioParameters parameters)
    {
        volume = parameters.volume;
        pitch = parameters.pitch;
        stereoPan = parameters.stereoPan;
        spatialBlend = parameters.spatialBlend;
        audioMixer = parameters.audioMixer;
        setMixer = parameters.setMixer;
    }

    [SerializeField]
    [Range(0,1)]
    private float volume;
    [SerializeField]
    [Range(0, 3)]
    private float pitch;
    [SerializeField]
    [Range(-1, 1)]
    private float stereoPan;
    [SerializeField]
    [Range(0, 1)]
    private float spatialBlend;
    [SerializeField]
    private AudioMixerGroup audioMixer;
    [SerializeField]
    private bool setMixer;

}