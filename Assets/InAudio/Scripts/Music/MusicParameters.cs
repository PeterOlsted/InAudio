using UnityEngine;

[System.Serializable]
public class MusicParameters
{
    public float Volume
    {
        get { return volume; }
        set { volume = Mathf.Clamp01(value); }
    }

    public MusicParameters()
    {
        volume = 1.0f;
    }

    public MusicParameters(MusicParameters parameters)
    {
        volume = parameters.volume;
    }

    public MusicParameters(float volume)
    {
        this.volume = Mathf.Clamp01(volume);
    }

    /// <summary>
    /// Reset all parameters to its default values
    /// </summary>
    public void Reset()
    {
        volume = 1.0f;
    }

    /// <summary>
    /// Copy values from another instance of this class
    /// </summary>
    /// <param name="parameters"></param>
    public void CopyFrom(MusicParameters parameters)
    {
        volume = parameters.volume;
    }

    [SerializeField]
    [Range(0, 1)]
    private float volume;
}