using UnityEngine;

public class Score : MonoBehaviour
{

    //Text that is TMPro 
    public TMPro.TextMeshProUGUI scoreText;
    public int score = 0;

    // Update is called once per frame
    void Update()
    {
        score += 10;
        scoreText.text = "Score: " + score;
        
    }
}
