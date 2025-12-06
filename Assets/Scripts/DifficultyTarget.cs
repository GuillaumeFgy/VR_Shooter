using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyTarget : MonoBehaviour
{
    [Header("Difficulty")]
    public DifficultyLevel difficulty = DifficultyLevel.Medium;

    [Header("Scene")]
    public string sceneToLoad = "HelloCardboard";

    [Header("Rotation")]
    public float rotationSpeed = 45f;   // degrees per second

    void Update()
    {
        // Rotate around Y-axis
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    public void OnHit()
    {
        // Set global difficulty values
        GameDifficulty.Apply(difficulty);

        // Load the actual game scene
        SceneManager.LoadScene(sceneToLoad);
    }
}
