using UnityEngine;

public class StopStressTest : MonoBehaviour
{
    public GameObject Prefab;
    public float TestLength = 5;
    public float IntervalStart;
    

    private float stopTime;
    private float timeSinceLast;

	// Use this for initialization
	void Start ()
	{
	    stopTime = Time.time + TestLength;

	    GameObject.Instantiate(Prefab);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    timeSinceLast += Time.deltaTime;
	    if (timeSinceLast > IntervalStart)
	    {
           // GameObject.Instantiate(Prefab);
	        timeSinceLast -= IntervalStart;
	    }
	    if (Time.time > stopTime)
	    {
	        gameObject.SetActive(false);
	    }
	}
}
