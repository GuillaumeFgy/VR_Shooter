using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxLives = 3;
    public GameObject[] hearts;

    [Header("FX")]
    public AudioClip hurtClip;
    public AudioClip deathClip;

    private int currentLives;
    private AudioSource audioSrc;
    private bool dead;

    void Awake()
    {
        currentLives = maxLives;
        audioSrc = GetComponent<AudioSource>();
        UpdateHearts();
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;

        currentLives -= amount;
        if (currentLives < 0) currentLives = 0;

        if (hurtClip) audioSrc.PlayOneShot(hurtClip);
        UpdateHearts();

        if (currentLives <= 0)
        {
            Die();
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < currentLives);
        }
    }

    void Die()
    {
        if (dead) return;
        dead = true;

        if (deathClip) audioSrc.PlayOneShot(deathClip);
        Debug.Log("PLAYER DIED");

        // Optional: trigger Game Over screen, restart, etc.
    }
}
