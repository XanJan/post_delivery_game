using System.Collections.Generic;
using System.Threading.Tasks;
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
    [SerializeField] protected observable_value_collection Obvc;
    /// <summary>
    /// Determines the maximum number of allowed active interactions.
    /// </summary>
    [SerializeField] protected int MaxInteractions = 1;
    /// <summary>
    /// Determines if the interactor can start new interactions.
    /// </summary>
    [SerializeField] protected bool CanInteract = true;
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
    public bool TryInteractStart(interactable_object interactable)
    {
        if(CanInteract && 
            (_activeInteractions.Count < MaxInteractions && !interactable.IsTrigger() || interactable.IsTrigger()) && 
                interactable.TryInteractStart(this))
        {
            if(!interactable.IsTrigger()){_activeInteractions.Push(interactable);}
            return true;
        } else return false;
    }
    /// <summary>
    /// Try to pop top of active interactions stack, returns true if the
    /// pop succeeded, false otherwise. 
    /// </summary>
    /// <returns>true on success, false on fail.</returns>
    public bool TryInteractEnd()
    {
        if(_activeInteractions.TryPop(out var topInteractable))
        {
            topInteractable.InteractEnd(this);
            return true;
        }
        else 
        {
            return false;
        }
    }
    /// <summary>
    /// Try to pop top of active interactions stack, returns true if the
    /// pop succeeded, false otherwise. Sets out variable to the popped interactable
    /// on success, null on fail.
    /// </summary>
    /// <returns>true on success, false on fail.</returns>
    public bool TryInteractEnd(out interactable_object interactable)
    {
        if(_activeInteractions.TryPop(out var topInteractable))
        {
            topInteractable.InteractEnd(this);
            interactable = topInteractable;
            return true;
        }
        else 
        {
            interactable = null;
            return false;
        }
    }

    /// <returns>Active interactions count.</returns>
    public int ActiveInteractionsCount {get{return _activeInteractions.Count;}}
    /// <summary>
    /// Pop the topmost interaction and peek the one underneath. Use in non trigger
    /// interactable objects to peek the last interaction. In trigger interactions,
    /// use TryPeek() directly instead.
    /// </summary>
    /// <param name="interaction">Last interactable on success, null on fail.</param>
    /// <returns>true on success, false on fail.</returns>
    public bool TryPeekLastInteraction(out interactable_object interaction)
    {
        if(_activeInteractions.TryPop(out var top)&&
        _activeInteractions.TryPeek(out var res))
        {
            interaction = res;
            _activeInteractions.Push(top);
            return true;
        } 
        else 
        {
            interaction = null ; 
            return false;
        }
    }
    /// <summary>
    /// Trypeek the interaction stack.
    /// </summary>
    /// <param name="interaction">Topmost interaction on success, null on fail.</param>
    /// <returns>true on success, false on fail.</returns>
    public bool TryPeekInteraction(out interactable_object interaction)
    {
        if(_activeInteractions.TryPeek(out var res))
        {
            interaction = res;
            return true;
        }
        else 
        {
            interaction = null;
            return false;
        }
    }
    /// <summary>
    /// Getter.
    /// </summary>
    /// <returns>Observable value collection attached to the interactor.</returns>
    public observable_value_collection GetPlayerObservableValueCollection(){return Obvc;}
    /// <summary>
    /// Try to end all previous active interactions in order. Stops if an interaction is not allowed to end.
    /// </summary>
    /// <param name="interactables">List of ended interactions.</param>
    /// <returns>True if all interactions</returns>
    public bool TryEndAllPreviousInteractions(out List<interactable_object> interactables)
    {
        interactables = new  List<interactable_object>();
        if(_activeInteractions.TryPop(out var temp))
        {
            bool stopped = false;
            while(_activeInteractions.Count > 0)
            {
                if(TryInteractEnd(out var res)) interactables.Add(res); 
                else {stopped = true;break;}
            }
            _activeInteractions.Push(temp);
            if(stopped) return false;
            else return true;
        }
        else 
        {
            return true;
        }
    }
    /// <summary>
    /// Ends all active interactions in order. Stops if an interaction is not allowed to end.
    /// </summary>
    public bool TryEndAllInteractions(out List<interactable_object> interactables)
    {
        interactables = new  List<interactable_object>();
        bool stopped = false;
        while(_activeInteractions.Count > 0)
        {
            if(TryInteractEnd(out var res)) interactables.Add(res); 
            else {stopped = true;break;}
        }
        if(stopped) return false;
        else return true;
    }
}
