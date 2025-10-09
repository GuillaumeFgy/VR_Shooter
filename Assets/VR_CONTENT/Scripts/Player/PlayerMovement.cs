using UnityEngine;



public class PlayerMovement : MonoBehaviour
{
    //THIS IS THE SPEED OF THE PLAYER
	public float speed=6.0f;
    //the reference to the main camera
	Transform cam;
	//the vector of movement obtained from input
	Vector3 movement;
    //the player's rigidbody
	Rigidbody playerRigidbody;
	
	
	public void Awake()
	{
        //set initial references
		playerRigidbody=GetComponent<Rigidbody>();
        cam = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
    }
	
	
	void FixedUpdate()
	{
		// Gets -1 o 0 o +1 without maps  A,D or arrows 
		float h=Input.GetAxis("Horizontal");
		float v=Input.GetAxis("Vertical");
		
        // obtains the direction of movement
		Vector3 director_movement=v* cam.forward+h* cam.right;
		director_movement[1]=0;
		director_movement=director_movement.normalized;		
		
		Move (director_movement);
		
		
		
	}	
	
	void OnCollisionEnter()
	{
		playerRigidbody.velocity=new Vector3(0,0,0);
	}
	
	
	//void Move(float h, float v)
	void Move(Vector3 director_movement)
	{
		
		//movement.Set(h,0f,v);
		movement=director_movement;
		movement=movement.normalized*speed*Time.deltaTime;
		
		
		playerRigidbody.MovePosition(transform.position+movement);  				
		
		
	}
	
	

	
	
}
