using UnityEngine;


public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public EnemyConfig config;

    [Header("Optional")]
    public Animator anim;
    public bool destroyOnDeath = true;
    public float destroyDelay = 2f;

    int hp;
    bool dead;

    void Awake()
    {
        hp = config ? config.maxHP : 50;
        // Make sure the tag is correct for PlayerShooting
        if (!CompareTag("Enemy")) gameObject.tag = "Enemy";
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;
        hp -= amount;

        // Apply pushback using Rigidbody if available
        if (config && config.pushBackForce > 0f)
        {
            var ctrl = GetComponent<EnemyController>();
            var rb = GetComponent<Rigidbody>();

            if (rb && ctrl && ctrl.target)
            {
                Vector3 pushDir = (transform.position - ctrl.target.position).normalized;
                pushDir.y = 0f; // keep push horizontal
                rb.AddForce(pushDir * config.pushBackForce, ForceMode.Impulse);
            }
        }

        if (hp <= 0)
        {
            Die();
        }
        else 
        {
            anim.SetTrigger("Hit");
        }
            

    }


    void Die()
    {
        if (dead) return;
        dead = true;

        // Trigger death animation
        if (anim)
        {
            anim.SetBool("Died", true);
        }

        var ctrl = GetComponent<EnemyController>();
        if (ctrl) ctrl.enabled = false;

        // Schedule destroy after delay (let animation play)
        if (destroyOnDeath)
            Destroy(gameObject, destroyDelay);
    }

}
