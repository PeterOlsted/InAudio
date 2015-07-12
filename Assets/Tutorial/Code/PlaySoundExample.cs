using UnityEngine;

public class PlaySoundExample : MonoBehaviour
{
    public InAudioNode soundExample;
	
	void OnEnable ()
	{
        //Play the audio attached to this game object.
        //Moving the game object will move the sound as well.
	    InAudio.Play(gameObject, soundExample);
	}
}
