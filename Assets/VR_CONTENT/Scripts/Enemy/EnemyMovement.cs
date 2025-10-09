using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{

    // min and max distances to set player's movemenet in the navmesh
	public float minDistanceToShoot;
	
    //these two variables are used to movement condition
	float distanceToPlayer;
    
    // player's scripts
    Transform player;
    PlayerHealth playerHealth;
    EnemyHealth enemyHealth;

    //mavmesh variables
    UnityEngine.AI.NavMeshAgent nav;   
    Vector3 lastNavDestination,outterDestination;
	bool triggerNewPos;

    //this is the animator of the foe
    Animator anim;

    void Awake ()
    {
        // initial references
        player = GameObject.FindGameObjectWithTag ("Player").transform;
        playerHealth = player.GetComponent <PlayerHealth> ();
        enemyHealth = GetComponent <EnemyHealth> ();
        nav = GetComponent <UnityEngine.AI.NavMeshAgent> ();
		nav.SetDestination (player.position);
        anim =GetComponent<Animator>() ;

    }

	

    void Update ()
    {

        // distances to the destination point (to know when it arrives)

        distanceToPlayer = (player.transform.position - transform.position).magnitude;

        // condition of movement imposed by health of the player and the foe
		if(enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
        {
        
			if(distanceToPlayer> minDistanceToShoot)
			{
                //move towards player
                nav.isStopped = false;
                nav.SetDestination (player.position);
				triggerNewPos=false;
                anim.SetBool("walking", true);

            }
            else
            {
                //stop moving
                nav.isStopped=true;
                               
                // look at player when stopped
                transform.LookAt(player.position);
                transform.rotation= Quaternion.Euler(0,transform.rotation.eulerAngles[1],0);
            }
			
            // this can be used to set the nav arround the player
			//lastNavDestination=nav.destination;
			//Random.Range(5,10)* (transform.position+new Vector3(Random.insideUnitCircle.x,0,Random.insideUnitCircle.y))
			
        }
        else
        {
            //stop moving if the condition is achieved
            nav.enabled = false;
            anim.SetBool("walking", false);
        }
    }
}
