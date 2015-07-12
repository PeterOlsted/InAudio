using InAudioSystem;
using UnityEngine;
using InAudioSystem.Internal;

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
                InAudioNode rootNode = InAudioInstanceFinder.DataManager.AudioTree;
                Debug.Log(rootNode.GetName);

                InAudio.PlayFollowing(gameObject, Node, Parameters);
                InAudioNode foundNode = TreeWalker.FindFirst(InAudioInstanceFinder.DataManager.AudioTree, node => node.GetName == "example name");
            }
        
    }
}
