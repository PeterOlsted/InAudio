using UnityEngine;

public class PlayEventExample : MonoBehaviour
{
    public InAudioEvent eventExample;

    void OnEnable()
    {
        //Post the event and thus play the audio attached to this game object.
        //Moving the game object will move the sound as well.
        InAudio.PostEvent(gameObject, eventExample);
    }
}
