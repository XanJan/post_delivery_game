using UnityEngine;
using UnityEngine.SceneManagement;

public class end_screen : MonoBehaviour
{
    public void returnToStart()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
