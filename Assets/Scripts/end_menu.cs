using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class end_screen : MonoBehaviour
{
    public TMPro.TextMeshProUGUI scoreText;
    public void returnToStart()
    {
        SceneManager.LoadSceneAsync(0);
    }
    public void Continue()
    {
        SceneManager.LoadSceneAsync(2);
    }

    private void Start()
    {
        updateScore();
    }

    public void updateScore()
    {
        scoreText.text = "Score: " + score_manager.Instance.totalPackageScore;
    }
}
