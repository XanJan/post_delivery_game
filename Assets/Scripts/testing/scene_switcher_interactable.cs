using UnityEngine;
using UnityEngine.SceneManagement;

public class scene_switcher_interactable : interactable_object
{
    [SerializeField] private int _sceneBuildIndex;
    public void OnTriggerHandler(interactor _)
    {
        SceneManager.LoadScene(_sceneBuildIndex,LoadSceneMode.Single);
    }
}
