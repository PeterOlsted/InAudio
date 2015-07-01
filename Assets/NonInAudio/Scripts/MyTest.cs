using UnityEngine;
using System.Collections;
using InAudioSystem;

public class MyTest : MonoBehaviour {

    public AudioParameters Parameters;
    public InAudioNode Node;
    public InMusicGroup Music;
    public InMusicNode Folder;

    

    // Use this for initialization
    void Start()
    {
    }
	
	// Update is called once per frame
	void Update () {
            if (Input.GetKeyDown(KeyCode.M))
            {
                Debug.Log("Play");

                InAudio.PlayFollowing(gameObject, Node, Parameters);
                
            }
        
    }
}
