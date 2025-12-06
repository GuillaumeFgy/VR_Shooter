using UnityEngine;
using TMPro;

public class MenuBestScores : MonoBehaviour
{
    [Header("Best Score Texts")]
    public TextMeshPro easyBestText;
    public TextMeshPro easyBestText2;
    public TextMeshPro mediumBestText;
    public TextMeshPro mediumBestText2;
    public TextMeshPro hardBestText;
    public TextMeshPro hardBestText2;

    void Start()
    {
        // Load best scores from PlayerPrefs
        int bestEasy = PlayerPrefs.GetInt("BestScore_Easy", 0);
        int bestMedium = PlayerPrefs.GetInt("BestScore_Medium", 0);
        int bestHard = PlayerPrefs.GetInt("BestScore_Hard", 0);

        if (easyBestText != null)
            easyBestText.text = bestEasy.ToString();
            easyBestText2.text = bestEasy.ToString();

        if (mediumBestText != null)
            mediumBestText.text = bestMedium.ToString();
            mediumBestText2.text = bestMedium.ToString();

        if (hardBestText != null)
            hardBestText2.text = bestHard.ToString();
    }
}
