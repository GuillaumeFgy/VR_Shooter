using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    //health variables
    public int startingHealth = 100;
    public int currentHealth;


    //this is the score that gives
    public int scoreValue = 10;

    public ParticleSystem hitParticles;
    
    // this is the 
	public UnityEngine.UI.Slider healthbar;

    Animator anim;
    AudioSource enemyAudio;
	BoxCollider boxcollider;
    bool isDead;
    Rigidbody enemyRigidBody;


    void Awake ()
    {
        //initial set up
        anim = GetComponent <Animator> ();

		boxcollider = GetComponent <BoxCollider> ();
		enemyRigidBody=GetComponent<Rigidbody>();
        isDead=false;
	
        currentHealth = startingHealth;
		healthbar.value=currentHealth/startingHealth;
    }


    void Update ()
    {
        
    }


    public void TakeDamage (int amount)
    {

        

        if(isDead)
            return;

        currentHealth -= amount;
            
        // get a hit
        hitParticles.Play();
		healthbar.value=(float)currentHealth/(float)startingHealth;
		
		// this occurs when the enemy dies
        if(currentHealth <= 0)
        {
            Death ();
        }
        else
        {
            // play the hit animation
            anim.SetTrigger("GetHit");
        }
        
		
    }


    void Death ()
    {
        // this is trigger when dying
        isDead = true;

		anim.SetTrigger ("Die");
        anim.SetBool("walking",false);
        ScoreManager.score+=scoreValue;
        enemyRigidBody.useGravity=true;
        transform.GetComponent<EnemyShooting>().enabled=false;
        Destroy(gameObject, 3f);
    }



    
}
