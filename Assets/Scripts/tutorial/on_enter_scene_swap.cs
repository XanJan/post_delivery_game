using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class on_enter_scene_swap : MonoBehaviour
{
    [SerializeField] int _sceneId;
    private void OnTriggerEnter(Collider other)
    {
        if(other.name=="interactable_wagon")
        {
            SceneManager.LoadScene(_sceneId,LoadSceneMode.Single);
        }
    }
}
