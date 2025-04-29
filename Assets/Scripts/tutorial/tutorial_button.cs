using UnityEngine;
using UnityEngine.Events;

public class tutorial_button : interactable_object
{
    [SerializeField] wagon_storage _wagon;


    [SerializeField] private UnityEvent _onPress;

    public void HandleOnTrigger(interactor context)
    {
        if(_wagon.IsFull())
        {
            _onPress.Invoke();
        }
    }
}
