using UnityEngine;
using System.Collections;

public class JumpToPositionInTime : MonoBehaviour {

	public Vector3 position, top,objective;
	
	public float slerpDown,slerpUp,slp;
	
	
	public SkinnedMeshRenderer[] myTutobjects;
	
	
	
	
	// Use this for initialization
	void Start () 
	{
		objective=top;
		transform.position=top;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		transform.position=Vector3.Lerp(transform.position,objective,slp);
		
		if((transform.position-position).magnitude>10)
		{
			foreach(SkinnedMeshRenderer go in myTutobjects)
			{
				go.enabled=false;
			}
		}
		else
		{
			foreach(SkinnedMeshRenderer go in myTutobjects)
			{
				go.enabled=true;
			}
		
		}
	
	}
	
	
	
	public void goTop()
	{
		objective=top;
		slp=slerpUp;
	}
	
	public void goPosition()
	{
		objective=position;
		slp=slerpDown;
	}
	
}
