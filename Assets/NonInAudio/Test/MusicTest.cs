using UnityEngine;

public class MusicTest : MonoBehaviour
{
    public double PlayInFuture = 0.0;
    public float volume = 1.0f;
    public float pitch = 1.0f;

    public InMusicGroup MusicRoot;
    public InMusicGroup MusicChild1;
    public InMusicGroup MusicChild2;


    private InMusicGroup[] MusicGroups = new InMusicGroup[0];

    void OnEnable()
    {
        MusicGroups = new []
        {
            MusicRoot, MusicChild1, MusicChild2
        };
    }

    private Vector2 scrollPos = new Vector2(300, 0);

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("", GUILayout.Width(400));
        GUILayout.EndVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(400));
        GUILayout.Label("Play");
        foreach (var node in MusicGroups)
        {
            if (GUILayout.Button(node.GetName))
            {
                InAudio.Music.Play(node);
            }
        }

        GUILayout.Label("Play At");
        foreach (var node in MusicGroups)
        {
            if (GUILayout.Button(node.GetName))
            {
                InAudio.Music.PlayAt(node, AudioSettings.dspTime + PlayInFuture);
            }
        }

        GUILayout.Label("Play Fadein");
        foreach (var node in MusicGroups)
        {
            if (GUILayout.Button(node.GetName))
            {
                InAudio.Music.PlayWithFadeIn(node, 2.0f);
            }
        }

        GUILayout.Label("Stop");
        foreach (var node in MusicGroups)
        {
            if (GUILayout.Button(node.GetName))
            {
                InAudio.Music.Stop(node);
            }
        }

        GUILayout.Label("Stop At");
        foreach (var node in MusicGroups)
        {
            if (GUILayout.Button(node.GetName))
            {
                InAudio.Music.StopAt(node, AudioSettings.dspTime + PlayInFuture);
            }
        }

        GUILayout.Label("Set Volume");
        foreach (var node in MusicGroups)
        {
            if (GUILayout.Button(node.GetName))
            {
                node.Volume = volume;
            }
        }

        GUILayout.Label("Set Pitch");
        foreach (var node in MusicGroups)
        {
            if (GUILayout.Button(node.GetName))
            {
                node.Pitch = pitch;
            }
        }

        GUILayout.Label("Flip mute");
        foreach (var node in MusicGroups)
        {
            if (GUILayout.Button(node.GetName))
            {
                node.Mute = !node.Mute;
            }
        }

        GUILayout.Label("Flip solo");
        foreach (var node in MusicGroups)
        {
            if (GUILayout.Button(node.GetName))
            {
                node.Solo = !node.Solo;
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
    }
}
