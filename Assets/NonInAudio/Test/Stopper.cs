using UnityEngine;
using System.Collections;

public class Stopper : MonoBehaviour {

    public InAudioEvent StopEvent;
    public InAudioEvent StartEvent;
    public float MinTime = 1;
    public float MaxTime = 4;

    private float endTime;

    // Use this for initialization
    void Start ()
    {
        endTime = Time.time + Random.Range(MinTime, MaxTime);
        InAudio.PostEvent(gameObject, StartEvent);
    }
	
	// Update is called once per frame
	void Update () {
	    if (Time.time > endTime)
	    {
            InAudio.PostEvent(gameObject, StopEvent);
	    }
	}
}
