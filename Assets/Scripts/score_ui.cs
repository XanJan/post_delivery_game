using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class score_ui : MonoBehaviour
{
    //public Text scoreText; 
    public TextMeshProUGUI scoreText;

    void Update()
    {
        scoreText.text = "Total Score: " + score_manager.Instance.totalPackageScore;
    }
}
