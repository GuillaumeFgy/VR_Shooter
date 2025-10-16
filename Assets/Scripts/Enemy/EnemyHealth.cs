using UnityEngine;


public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public EnemyConfig config;

    [Header("Optional")]
    public Animator anim;
    public string deathTrigger = "Die";
    public bool destroyOnDeath = true;
    public float destroyDelay = 2f;

    int hp;
    bool dead;

    void Awake()
    {
        hp = config ? config.maxHP : 50;
        // Make sure the tag is correct for PlayerShooting
        if (!CompareTag("enemy")) gameObject.tag = "enemy";
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;
        hp -= amount;
        if (hp <= 0) Die();
    }

    void Die()
    {
        dead = true;

        if (anim && !string.IsNullOrEmpty(deathTrigger))
        {
            anim.SetTrigger(deathTrigger);
        }


        if (config && config.deathVfx) 
        {
            Instantiate(config.deathVfx, transform.position, Quaternion.identity);
        }


        if (config && config.deathSfx) 
        {
            AudioSource.PlayClipAtPoint(config.deathSfx, transform.position);
        }
           
        // Disable collisions & logic scripts
        var col = GetComponent<Collider>(); if (col) col.enabled = false;
        var ctrl = GetComponent<EnemyController>(); if (ctrl) ctrl.enabled = false;

        if (destroyOnDeath) Destroy(gameObject, destroyDelay);
    }
}
