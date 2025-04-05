using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public AudioMixer audio;
    public TMP_Dropdown resDropdown;

    Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            if(i == 0 || i == 19 || i == 24)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);
            }

            if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }
        
        resDropdown.AddOptions(options);
        resDropdown.value = currentResIndex;
        resDropdown.RefreshShownValue();
    }
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Debug.Log("EXIT");
        Application.Quit();
    }

    public void SetRes(int resIndex)
    {
        Resolution res = resolutions[resIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetVolume(float vol)
    {
        Debug.Log(vol);
        audio.SetFloat("volume", vol);
    }

    public void FullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }
}
