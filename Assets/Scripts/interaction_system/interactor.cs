using System.Collections.Generic;
using UnityEngine;
// Authors : Alexander Lisborg

/// <summary>
/// General interactor behaviour. Extend to create a specific interactor and
/// controll the behaviour through calls to BeginInteraction(interactable_object) 
/// and TryEndInteraction().
/// 
/// Behaviour
/// Interactions can be begun and then later ended. Beginning an interaction invokes
/// the UnityEvent onBeginInteraction attached to the interactable object. Ending an
/// interaction runs the corresponding onEndInteraction. Active interactions are
/// stored in a stack. This enables interactables to interact with interactables
/// that are on the stack. An interactable object can be set to istrigger, in which 
/// case both onBeginInteraction and onEndInteraction is run right after one another 
/// starting with the onBeginInteraction. 
/// </summary>
public abstract class interactor : MonoBehaviour
{
    /// <summary>
    /// An observable value collection is attached to the interactor
    /// so that interactables can interact with it.
    /// </summary>
    [SerializeField]private observable_value_collection _obvc;
    /// <summary>
    /// The interactor can begin and end interactions. All active interactions
    /// are stored in this stack. Pop the top to get the top active interaction.
    /// Note that interactions are pushed onto the stack before the begin event
    /// executes. interactions with IsTrigger interactables do not update the
    /// stack.
    /// </summary>
    protected Stack<interactable_object> _activeInteractions;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public interactor()
    {
        _activeInteractions = new Stack<interactable_object>();
    }
    /// <summary>
    /// Begin interaction with interactable. Push interactable to active 
    /// interactables stack. If the interactable IsTrigger, does not modify
    /// and executes end interaction event directly after begin interaction 
    /// call.
    /// </summary>
    /// <param name="interactable"></param>
    public void BeginInteraction(interactable_object interactable)
    {
        if(!interactable.IsTrigger()){_activeInteractions.Push(interactable);}
        interactable.BeginInteraction(this);
        if(interactable.IsTrigger()){interactable.EndInteraction(this);}
    }
    /// <summary>
    /// Try to pop top of active interactions stack, returns true if the
    /// pop succeeded, false otherwise.
    /// </summary>
    /// <returns>true on success, false on fail.</returns>
    public bool TryEndInteraction()
    {
        if(_activeInteractions.TryPop(out var topInteractable))
        {
            topInteractable.EndInteraction(this);
            
            return true;
        }
        else 
        {
            return false;
        }
    }
    /// <summary>
    /// Cancel interaction without invoking end interaction event.
    /// Pops top of active interactions stack.
    /// </summary>
    /// <param name="interactable"></param>
    public void CancelInteraction(interactable_object interactable)
    {
        _activeInteractions.Pop();
        interactable.RemoveActiveInteractor(this);
    }

    /// <returns>active interactions count.</returns>
    public int ActiveInteractionsCount(){return _activeInteractions.Count;}
    /// <summary>
    /// Pop the topmost interaction and peek the one underneath. Use in non trigger
    /// interactable objects to peek the last interaction. In trigger interactions,
    /// use TryPeek() directly instead.
    /// </summary>
    /// <returns>True if peek succeeded, false if not.</returns>
    public bool TryPeekLastInteraction(out interactable_object interaction)
    {
        if(_activeInteractions.TryPop(out var top)&&
        _activeInteractions.TryPeek(out var res))
        {
            interaction = res;
            _activeInteractions.Push(top);
            return true;
        } else {interaction = null ; return false;}
    }
    /// <summary>
    /// Getter.
    /// </summary>
    /// <returns>Observable value collection attached to the interactor.</returns>
    public observable_value_collection GetPlayerObservableValueCollection(){return _obvc;}
    /// <summary>
    /// Try pop active interactions stack.
    /// </summary>
    /// <param name="result">Popped interactable object.</param>
    /// <returns>true on success, false on fail.</returns>
    public bool TryPopInteraction( out interactable_object result )
    {
        bool success = _activeInteractions.TryPop(out var res);
        result = res;
        return success;
    }
    /// <summary>
    /// Push interactable object onto active interactions stack.
    /// </summary>
    /// <param name="interactable">Interactable object to push.</param>
    public void PushInteraction(interactable_object interactable)
    {
        _activeInteractions.Push(interactable);
    }
    /// <summary>
    /// Ends all active interactions in order.
    /// </summary>
    public void EndAllPreviousInteractions()
    {
        if(_activeInteractions.TryPop(out var temp))
        {
            for(int i= 0 ; i < _activeInteractions.Count ; i++){TryEndInteraction();}
            _activeInteractions.Push(temp);
        }
    }
}
