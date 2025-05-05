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
    [SerializeField] private string _defaultInteractionText = "Press %interactButton% to interact";
    /// <summary>
    /// Triggered by interactor when it starts an interaction with this interactable.
    /// </summary>
    [SerializeField] private UnityEvent<interactor> _onInteractStart ;
    /// <summary>
    /// Triggered by interactor when it ends an interaction with this interactable.
    /// </summary>
    [SerializeField] private UnityEvent<interactor> _onInteractEnd ;
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
    private UnityEvent<interactor, interactable_object> _onForceInteractEnd = new UnityEvent<interactor, interactable_object>();
    /// <summary>
    /// If false, interactors cannot interact with this object. Already active interactions
    /// can still be ended.
    /// </summary>
    [SerializeField] public bool AllowInteractions = true;
    /// <summary>
    /// Active interactors.
    /// </summary>
    protected List<interactor> _activeInteractors = new List<interactor>();
    /// <summary>
    /// Default interaction text. Not settable, for changing the text, use InteractionText;
    /// </summary>
    protected string DefaultInteractionText{get{return _defaultInteractionText;}}
    void Start()
    {
        _onInteractStart.AddListener(OnInteractStart);
        _onInteractEnd.AddListener(OnInteractEnd);
        _onForceInteractEnd.AddListener(OnForceInteractEnd);
        
    }
    /// <summary>
    /// Invoke onBeginInteraction if not full. If the interactable is at max active interactors, the 
    /// interaction is canceled via the interactor. If istrigger, does not update the active 
    /// interactors stack.
    /// </summary>
    /// <param name="interactor">The interactor attempting interaction with this interactable.</param>
    public bool TryInteractStart(interactor interactor)
    {
        // Istrigger simply trigger events...
        if(_isTrigger && AllowInteractions)
        {
            _onInteractStart?.Invoke(interactor);
            _onInteractEnd?.Invoke(interactor);
            return true;
        }
        // If not trigger, check if interaction is possible...
        else if(_activeInteractors.Count < _maxNumberOfInteractors && _activeInteractors.Contains(interactor) == false && AllowInteractions)
        {
            _activeInteractors.Add(interactor);
            _onInteractStart?.Invoke(interactor);
            return true;
        } 
        else
        {
            return false;
        }
    }
    /// <summary>
    /// Invoke onEndInteraction.
    /// </summary>
    /// <param name="interactor">The interactor to end the interaction with.</param>
    public void InteractEnd(interactor interactor)
    { 
        if(!_isTrigger){_activeInteractors.Remove(interactor);} // Cannot end interactions with onTrigger
        _onInteractEnd?.Invoke(interactor);
    }
    public void ForceInteractEnd(interactor interactor, interactable_object source)
    {
        _onForceInteractEnd?.Invoke(interactor,source);
        if(!_isTrigger){_activeInteractors.Remove(interactor);} // Cannot end interactions with onTrigger
    }
    /// <summary>
    /// Getter.Processes the input string to replace special substrings with strings depending on the interactor.
    /// </summary>
    /// <returns>Text that describes the interaction.</returns>
    public string GetInteractionText(interactor context)
    {
        string s = GetInteractionTextOverride(context);
        if(s.Contains("%interactButton%"))
        {
            string ret = s.Replace("%interactButton%", context.GetPlayerObservableValueCollection().GetObservableString("interactButton").Value);
            return ret;
        } else {
            return s;
        }
    }    
    /// <summary>
    /// Getter.
    /// </summary>
    /// <returns>Wether the interactable object has enabled is trigger or not.</returns>
    public bool IsTrigger(){return _isTrigger;}
    public interactor[] GetInteractors(){return _activeInteractors.ToArray();}
    protected virtual void OnForceInteractEnd(interactor context, interactable_object source){}
    protected virtual void OnInteractStart(interactor context){}
    protected virtual void OnInteractEnd(interactor context){}
    protected virtual string GetInteractionTextOverride(interactor context){return _defaultInteractionText;}
}
