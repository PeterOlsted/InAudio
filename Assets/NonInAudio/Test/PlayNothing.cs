using UnityEngine;
using System.Collections;

public class PlayNothing : MonoBehaviour
{
    public InMusicGroup group;

	// Use this for initialization
	void OnEnable ()
	{
	    InAudio.Music.Play(group);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
