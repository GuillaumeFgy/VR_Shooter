using UnityEngine;
using System.Collections;

public class InteractionCanvas : MonoBehaviour {

	public float distanceToActivate;
	public Vector2 jumpingDirection;
	

	private Canvas thisCanvas;
	private GameObject player;
	private Rigidbody rigidBodyPlayer;
	
	// Use this for initialization
	
	void Start () {
		thisCanvas=transform.GetChild(0).GetComponent<Canvas>();
		player=GameObject.FindGameObjectWithTag("Player");
		rigidBodyPlayer=player.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		float distance=(transform.position-player.transform.position).magnitude;
		
		if(distance<distanceToActivate)
		{
				thisCanvas.enabled=true;
		}
		else
		{
				thisCanvas.enabled=false;
		}
	}
	
	public void makePlayerJump()
	{
		
		rigidBodyPlayer.velocity= rigidBodyPlayer.transform.GetChild(0).forward*jumpingDirection.x+Vector3.up*jumpingDirection.y;
	}
	
	
	public void pushObject(Rigidbody objectRigidbody)
	{
		
		objectRigidbody.velocity=player.transform.forward*jumpingDirection.x;
	}
}
