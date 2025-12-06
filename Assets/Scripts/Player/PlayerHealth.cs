using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        Handheld.Vibrate();

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

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SaveBestScoreForCurrentDifficulty();
        }

        StartCoroutine(ReturnToMenu());
    }


    private IEnumerator ReturnToMenu()
    {
        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadScene("Menu");
    }
    public bool AddLife(int amount)
    {
        if (dead) return false;

        int newLives = Mathf.Clamp(currentLives + amount, 0, maxLives);
        bool increased = newLives > currentLives;

        currentLives = newLives;
        UpdateHearts();

        return increased;
    }

}
