using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    [Header("Animation")]
    [Tooltip("Time between each +1 step when the score is catching up.")]
    public float stepInterval = 0.03f;

    private int targetScore = 0;      // real score
    private int displayedScore = 0;   // what the UI currently shows
    private float stepTimer = 0f;

    public int CurrentScore => targetScore;

    void Awake()
    {
        // Simple singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        targetScore = 0;
        displayedScore = 0;
        UpdateUI();
    }

    void Update()
    {
        if (displayedScore < targetScore)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                stepTimer = stepInterval;
                displayedScore++;
                UpdateUI();
            }
        }
    }

    public void AddScore(int amount)
    {
        if (amount <= 0) return;

        targetScore += amount;
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = displayedScore.ToString();
        }
    }


    string GetBestKeyForDifficulty(DifficultyLevel difficulty)
    {
        // Keys like: "BestScore_Easy", "BestScore_Medium", "BestScore_Hard"
        return $"BestScore_{difficulty}";
    }

    /// <summary>
    /// Save current score as best if it's higher than the existing best
    /// for the current difficulty.
    /// </summary>
    public void SaveBestScoreForCurrentDifficulty()
    {
        DifficultyLevel diff = GameDifficulty.current;
        string key = GetBestKeyForDifficulty(diff);

        int previousBest = PlayerPrefs.GetInt(key, 0);
        if (CurrentScore > previousBest)
        {
            PlayerPrefs.SetInt(key, CurrentScore);
            PlayerPrefs.Save(); // make sure it’s written to disk
            Debug.Log($"New best score for {diff}: {CurrentScore}");
        }
        else
        {
            Debug.Log($"Score {CurrentScore} did not beat best ({previousBest}) for {diff}");
        }
    }

    /// <summary>
    /// Helper you can use anywhere to get best score for a given difficulty.
    /// </summary>
    public int GetBestScore(DifficultyLevel difficulty)
    {
        string key = GetBestKeyForDifficulty(difficulty);
        return PlayerPrefs.GetInt(key, 0);
    }
}
