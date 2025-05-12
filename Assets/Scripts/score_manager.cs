using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class score_manager : MonoBehaviour
{
    public static score_manager Instance;
    public int totalPackageScore = 0; 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); 
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    void Start()
    {
        SceneManager.sceneLoaded +=OnSceneLoad;
    }
    public void OnSceneLoad(UnityEngine.SceneManagement.Scene s, LoadSceneMode mode)
    {
        if(s.name!="end_screen_revamped")
        {
            Debug.Log("Resetting score");
            totalPackageScore = 0;
        }


    public void AddScore(int point)
    {
        this.totalPackageScore += point;
        Debug.Log("Total Package Score: " + totalPackageScore);
    }

}
