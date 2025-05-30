using UnityEngine;

public class score_manager : MonoBehaviour
{
    public static score_manager Instance;
    public int totalPackageScore = 0; 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    public void AddScore()
    {
        totalPackageScore++;
        Debug.Log("Total Package Score: " + totalPackageScore);
    }
}
