using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    //shooting parameters
	public int damagePerShot = 20;
	public float timeBetweenBullets = 0.15f;
	public GameObject refNozzle;

    //this is used to generate the shooting effect
	private float elapsed;
	private Ray shootRay;
	private RaycastHit shootHit;
	private AudioSource gunAudio;
	private float effectsDisplayTime = 0.2f;
    Animator anim;
    LineRenderer linRend;

    private GameObject player;
	private PlayerHealth playerHealthScript;
    private EnemyMovement enemyMovScript;

    // these are the bullets and bullet variables
    public int initialBullets = 3;
    int numberOfBullets = 3;
    
    // public bool to know if the guy is reloading
    bool reloading = false;
    public float reloadTime = 1.6f;

    //audioclips for shooting and reloading
    public AudioClip shootClip, realoadCLip;

    public void Awake ()
	{

        //set up the variables
		gunAudio = GetComponent<AudioSource> ();
		player=GameObject.FindGameObjectWithTag("Player");
		playerHealthScript=player.GetComponent<PlayerHealth>();
        enemyMovScript = GetComponent<EnemyMovement>();
        anim = GetComponent<Animator>();
        linRend = GetComponent<LineRenderer>();

        //set the initial number of bullets
        numberOfBullets = initialBullets;



    }
	
	
	void FixedUpdate ()
	{
		elapsed += Time.deltaTime;
		
        // shoot if the distance to player is lower that the one imposed
		if(  (transform.position-player.transform.position).magnitude< enemyMovScript.minDistanceToShoot && elapsed>timeBetweenBullets && playerHealthScript.currentHealth>0 && reloading==false)
		{
			Shoot ();
		}
		
		
	}
	
	void disableEffects()
    {
        linRend.enabled = false;
    }


    void Shoot ()
	{
		elapsed = 0f;

        gunAudio.clip = shootClip;
        gunAudio.Play ();

        // add line effects and others
        anim.SetTrigger("Shoot");


        //set parameters of the line-renderer
        linRend.SetPosition(0, refNozzle.transform.position);
        linRend.SetPosition(1, player.transform.position-new Vector3(0,0.5f,0));
        //shooting effects
        linRend.enabled = true;

        //dissable shooting effects
        Invoke("disableEffects", effectsDisplayTime);

        //getplayerhit
        playerHealthScript.TakeDamage(damagePerShot);


        //reloadingCOndition
        numberOfBullets -= 1;
        if (numberOfBullets <= 0)
        {
            numberOfBullets = initialBullets;
            anim.SetTrigger("Reload");
            reloading = true;

            Invoke("stopReloading", reloadTime);

        }
    }

    void stopReloading()
    {
        reloading = false;
        gunAudio.clip = realoadCLip;
        gunAudio.Play();
    }

}
