using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class computer_interactable : interactable_object
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_InputField _field;
    private interactor _current;
    protected override void OnInteractStart(interactor context)
    {
        Time.timeScale=0;
        _panel.SetActive(true);
        _field.gameObject.SetActive(true);
        _field.text = _activeInteractors[0].GetPlayerObservableValueCollection().GetObservableString("playerName").Value;
        var submitEvent=new TMP_InputField.SubmitEvent();
        submitEvent.AddListener(SubmitName);
        _field.onEndEdit = submitEvent;
        
        if(context.TryGetComponent<PlayerInput>(out var playerInput))
        {
            playerInput.actions["interact"].canceled+=HandleInteractCancel;
        }
    }
    public void HandleInteractCancel(InputAction.CallbackContext callbackContext)
    {
        if(_activeInteractors[0].TryGetComponent<PlayerInput>(out var playerInput))
        {
            playerInput.SwitchCurrentActionMap("UI");
            playerInput.actions["interact"].canceled-=HandleInteractCancel;
        }
    }



    public void SubmitName(string s)
    {
        _activeInteractors[0].GetPlayerObservableValueCollection().InvokeString("playerName",s);
        Time.timeScale=1;
        _panel.SetActive(false);
        _field.gameObject.SetActive(false);
        if(_activeInteractors[0].TryGetComponent<PlayerInput>(out var playerInput))
        {
            playerInput.SwitchCurrentActionMap("Player");
        }
        bool b = _activeInteractors[0].TryInteractEnd();
    }

    public void SubmitName()
    {
        SubmitName(_field.text);
    }
}
