using UnityEngine;
using UnityEngine.UI; //used for the sliders and UI components
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{

    //this are the health parameters
    public int startingHealth = 100;
    public int currentHealth;

    // health slider
    public Slider healthSlider;

    // used to simulate the hitting effect
    public Image damageImage;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);
	public bool isDead;
   
    // locla variables of player's movement and shooting
    PlayerMovement playerMovement;
    PlayerShooting playerShooting;

        
    bool damaged;


    public void Awake ()
    {
  
        playerMovement = GetComponent <PlayerMovement> ();
        playerShooting = GetComponentInChildren <PlayerShooting> ();
		//playerRender = GetComponentInChildren <Renderer> ();
        currentHealth = startingHealth;
        
        //player body can't be seen
        isDead=false;
		
    }


    void Update ()
    {
        //show image hit when it gets hit
        if(damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;

        // update slider
        healthSlider.value = (float)currentHealth / (float)startingHealth;
    }


    public void TakeDamage (int amount)
    {
        damaged = true;

        currentHealth -= amount;

        if(currentHealth <= 0 && !isDead)
        {
            Death ();
        }

       

    }


    void Death ()
    {
        isDead = true;

        //playerAudio.clip = deathClip;
        //playerAudio.Play ();

        playerMovement.enabled = false;
        playerShooting.enabled = false;
        
        //fall to ground
        GetComponent<Rigidbody>().constraints= RigidbodyConstraints.None;

        transform.rotation =Quaternion.Euler(-3f,0,-3f);

        Invoke("RestartLevel",6.0f);
    }


    public void RestartLevel()
	{
		SceneManager.LoadScene(0);
    }
    

    


}
