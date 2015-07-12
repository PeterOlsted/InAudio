using InAudioSystem.Runtime;
using UnityEngine;
public class ParameterExample : MonoBehaviour
{
    public InAudioNode gunshot;
    public InAudioNode footstep;

    public AudioParameters audioParameters;

    public InPlayer player;

    void OnEnable()
    {
        player = InAudio.Play(gameObject, gunshot);
        player.OnCompleted += (GO, audioNode) => Debug.Log("Node " + audioNode.GetName + " has finished playing");

        //OR


    }

    void Update()
    {
        //if(Input.GetKey(KeyCode.H))
            player.SetFromAudioParameters(audioParameters);
    }
}

public class ParameterExample2 : MonoBehaviour
{
    public InAudioNode gunshot;
    public InAudioNode footstep;

    public AudioParameters audioParameters;

    public InPlayer player;

    void OnEnable()
    {
        player = InAudio.Play(gameObject, gunshot);
        player.Volume = 0.5f;
        player.SpatialBlend = 0.2f;
        player.PanStereo = 0.0f;
        player.AudioMixer = null;
        player.OnCompleted += (GO, audioNode) => Debug.Log("Node " + audioNode.GetName + " has finished playing");

        //OR

       
    }

    void Update()
    {
        player.SetFromAudioParameters(audioParameters);
    }
}
