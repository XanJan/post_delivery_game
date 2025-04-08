using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An interactable object. Attach directly for UnityEvent functionality or
/// extend to specify behaviour for a specific subclass of interactable objects.
/// </summary>
public class interactable_object : MonoBehaviour
{
    /// <summary>
    /// Text that describes the interaction.
    /// </summary>
    [SerializeField] protected string _defaultInteractionText = "Press %interactButton% to interact";
    /// <summary>
    /// Triggered by interactor when it starts an interaction with this interactable.
    /// </summary>
    [SerializeField] private UnityEvent<interactor> _onInteractStart;
    /// <summary>
    /// Triggered by interactor when it ends an interaction with this interactable.
    /// </summary>
    [SerializeField] private UnityEvent<interactor> _onInteractEnd;
    /// <summary>
    /// IsTrigger interactable objects does not affect the active interactions stack
    /// nor the active interactors list and only triggers begin interaction and end
    /// interaction events whenever an interaction is begun.
    /// </summary>
    [SerializeField] private bool _isTrigger;
    /// <summary>
    /// Specifies how many interacors to allow simultaneously. When that number of
    /// interactors are actively interacting with this interactable, stop all other
    /// incoming interaction attempts.
    /// </summary>
    [SerializeField] private int _maxNumberOfInteractors = 1;
    /// <summary>
    /// Active interactors.
    /// </summary>
    protected List<interactor> _activeInteractors = new List<interactor>();
    /// <summary>
    /// The interaction text used in game, this can be modified to deviate from the original 
    /// interaction text depending on the situation. Accessible through public getter.
    /// </summary>
    protected string interactionText;
    void Awake()
    {
        interactionText = _defaultInteractionText;
    }
    /// <summary>
    /// Invoke onBeginInteraction if not full. If the interactable is at max active interactors, the 
    /// interaction is canceled via the interactor. If istrigger, does not update the active 
    /// interactors stack.
    /// </summary>
    /// <param name="interactor">The interactor attempting interaction with this interactable.</param>
    public void BeginInteraction(interactor interactor)
    {
        if(_activeInteractors.Count < _maxNumberOfInteractors && _activeInteractors.Contains(interactor) == false)
        {
            if(!_isTrigger){_activeInteractors.Add(interactor);}
            _onInteractStart?.Invoke(interactor);
        } else{interactor.CancelInteraction(this);}
    }
    /// <summary>
    /// Invoke onEndInteraction
    /// </summary>
    /// <param name="interactor"></param>
    public void EndInteraction(interactor interactor)
    {
        if(!_isTrigger){RemoveActiveInteractor(interactor);}
        _onInteractEnd?.Invoke(interactor);
    }
    /// <summary>
    /// Getter.
    /// </summary>
    /// <returns>Text that describes the interaction.</returns>
    public string GetInteractionText(interactor interactor)
    {
        if(interactionText.Contains("%interactButton%"))
        {
            string ret = interactionText.Replace("%interactButton%",
                                        interactor.GetPlayerObservableValueCollection().GetObservableString("interactButton").Value);
            return ret;
        } else {
            return interactionText;
        }
    }
    /// <summary>
    /// Getter.
    /// </summary>
    /// <returns>Wether the interactable object has enabled is trigger or not.</returns>
    public bool IsTrigger(){return _isTrigger;}
    /// <summary>
    /// Removes active interactor from list. Use interactor.TryEndInteraction instead to end interactions.
    /// </summary>
    /// <param name="interactor">The interactor to remove from active interactors list.</param>
    public void RemoveActiveInteractor(interactor interactor){_activeInteractors.Remove(interactor);}
    /// <summary>
    /// Add active interactor. Use interactor.BeginInteraction instead for beginning interactions.
    /// </summary>
    /// <param name="interactor"></param>
    public void AddActiveInteractor(interactor interactor){_activeInteractors.Add(interactor);}
}
