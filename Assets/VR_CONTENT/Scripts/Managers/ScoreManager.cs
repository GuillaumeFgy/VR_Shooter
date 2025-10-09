using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static int score;
    public Text textScore1;


	

    void Awake ()
    {
    
        score = 0;
    }

    
    void Update ()
    {
		textScore1.text = " " + score;
		
    }
}
