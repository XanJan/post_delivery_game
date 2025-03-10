using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public AudioMixer audio;
   public void PlayGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Debug.Log("EXIT");
        Application.Quit();
    }

    public void SetVolume(float vol)
    {
        Debug.Log(vol);
        audio.SetFloat("volume", vol);
    }
}
