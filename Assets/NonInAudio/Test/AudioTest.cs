using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public InAudioNode SimpleSound;
    public InAudioNode RandomSound;
    public InAudioNode SequenceSound;
    public InAudioNode MultiSound;

    public InAudioNode SoundPitch;
    public InAudioNode SoundVolume;
    public InAudioNode SoundSpatialBlend;
    public InAudioNode SoundLoop;

    public InAudioNode InstanceLimiting;
    public InAudioNode InstanceLimitingStealNewest;
    public InAudioNode InstanceLimitingStealOldest;

    public InAudioNode RandomSoundNoRepeat;

    public InAudioNode RandomSoundSkipTime;

    public InAudioNode FolderVolume;

    private InAudioNode[] nodes = new InAudioNode[0];

    void OnEnable()
    {
        nodes = new[]
        {
            SimpleSound,
            RandomSound,
            SequenceSound,
            MultiSound,
            SoundPitch,
            SoundVolume,
            SoundSpatialBlend,
            SoundLoop,
            InstanceLimiting,
            InstanceLimitingStealNewest,
            InstanceLimitingStealOldest,
            RandomSoundNoRepeat,
            RandomSoundSkipTime,
            FolderVolume,
        };
    }

    private Vector2 scrollPos;

    void OnGUI()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        foreach (var node in nodes)
        {
            if (GUILayout.Button(node.GetName))
            {
                InAudio.Play(gameObject, node);
            }
        }
        GUILayout.EndScrollView();
    }
}
