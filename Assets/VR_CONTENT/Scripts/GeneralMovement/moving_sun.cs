using UnityEngine;
using System.Collections;



public class moving_sun : MonoBehaviour {

	//private CharacterMotor motor;
	public bool checkAutoMove = true;
	public bool checkAutoRotate = false;
	public bool autoLook =true;
	public Transform player;
	
	public float rspeed;
	public float dspeed;

	public float radius;
	private Transform tx;
	
	public float initialAng =Mathf.PI/2;
	
	public Vector3 initialPosition;
	
	// Use this for initialization
	void Start () 
	{
		tx =transform;
		
	}
	
		
	// Update is called once per frame
	void FixedUpdate()
	{
		// Get the input vector from keyboard or analog stick
		Vector3 directionVector;
		
		if (!checkAutoMove) { 
			directionVector = new Vector3(0, 0, 0);
		} else { 
			directionVector = new Vector3(Mathf.Cos (dspeed*Time.time-initialAng), 0, Mathf.Sin (dspeed*Time.time-initialAng));
			
			
		}
		
		//var rot = transform.rotation;
		//transform.rotation = rot * Quaternion.Euler(0, 1, 0); 
		//transform.rotation=head.transform.rotation;
		
		
		if(checkAutoRotate)
		{
			if(Time.time>0.1)
			{
				//audio.PlayOneShot(sound);
				//pow=(pow+0.1f)*1.005f;
				// Apply the direction to the CharacterMotor
				tx.Rotate(Vector3.up * rspeed*Time.deltaTime, Space.World);
				
				
				
				//motor.inputJump = Input.GetButton("Jump");
			}
		}
		
		
		tx.position=initialPosition+radius*directionVector;
		
		
		if(autoLook=true)
		{
			transform.rotation = Quaternion.LookRotation(transform.position - player.position);
		 //tx.rotation=tx.rotation*Quaternion.Euler(0,0,0);
		}
		
		
		
		
		
	}
	
}
