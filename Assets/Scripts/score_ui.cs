using UnityEngine;
using UnityEngine.UI;

public class score_ui : MonoBehaviour
{
    public Text scoreText; 

    void Update()
    {
        scoreText.text = "Total Score: " + score_manager.Instance.totalPackageScore;
    }
}
