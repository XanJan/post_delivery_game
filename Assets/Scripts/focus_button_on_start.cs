using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class focus_button_on_start : MonoBehaviour
{
    [SerializeField] private Button _button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button.Select();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
