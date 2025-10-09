using UnityEngine;
using System.Collections;

public class SelfDestroyTime : MonoBehaviour {

	public float elapsed, timeToDestroy;
	
	// Use this for initialization
	void Start () 
	{
		elapsed=0;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		elapsed+=Time.fixedDeltaTime;
		
		if(elapsed>timeToDestroy)
		{
			Object.Destroy(gameObject);
		}
		
		
	}
}
