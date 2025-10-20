using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject startPanel;

    [Header("Input")]
    public SoundDetector clapDetector;

    // One-shot flag to suppress the start panel on the *next* load
    private static bool suppressPanelNextLoad = false;

    private bool gameRunning = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // If we just reloaded due to a start/restart, skip the panel and run
        if (suppressPanelNextLoad)
        {
            suppressPanelNextLoad = false;
            gameRunning = true;
            ShowStartPanel(false);   // unpause
            return;
        }

        // Normal boot: show panel and pause until clap
        ShowStartPanel(true);
    }

    void Update()
    {
        if (!clapDetector) return;

        if (clapDetector.ConsumeClap())
        {
            if (!gameRunning)
            {
                StartGame();   // reload scene, then auto-run without panel
            }
            else
            {
                RestartGame(); // reload scene, then auto-run without panel
            }
        }
    }

    public void OnPlayerDied()
    {
        Debug.Log("Player died -> waiting for clap to restart...");
        gameRunning = false;
        ShowStartPanel(true);   // show panel + pause; next clap will reload
    }

    private void StartGame()
    {
        // Reload and auto-skip panel on load
        suppressPanelNextLoad = true;
        Time.timeScale = 1f; // ensure not paused during load
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void RestartGame()
    {
        // Same behavior as StartGame()
        suppressPanelNextLoad = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ShowStartPanel(bool show)
    {
        if (startPanel) startPanel.SetActive(show);
        Time.timeScale = show ? 0f : 1f;  // Pause while panel shown
    }
}
