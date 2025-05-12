//----------------------------------------------------------------------------
// Authors : Alexander Lisborg
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Event based solution for passing valus to animators. For each float and bool 
///  value in the attached ObservableValueCollection, checks if the equivalent
/// parameters are present in the animator, if so, attaches event handlers that
/// pass those values to a set of animators. NOT THREAD SAFE.
/// </summary>
public class animator_handler : MonoBehaviour
{
    /// <summary>
    /// The Animator(s) to pass values to. Prints debug message if null. 
    /// </summary>
    [SerializeField] private List<Animator> _animators;
    /// <summary>
    /// The ObservableValueCollection to observe. Prints debug message if null.
    /// </summary>
    [SerializeField] private observable_value_collection _observableValueCollection;
    /// <summary>
    /// Using, the animator as key, get the associated parameters. Initialized in Start().
    /// </summary>
    private Dictionary<Animator,string[]> _parametersToListenToDictionary = new Dictionary<Animator,string[]>();
    
    /// <summary>
    /// Tries to attach handlers to all events in the ObservableValueCollection.
    /// ObservableValueCollection is initialized at Awake(), the attachment
    /// of event handlers is done at Start() since Start() runs after Awake().
    /// </summary>
    void Start()
    {
        // Null check. If serializable fields are null, does not initialize events.
        // Basically, whole script has no effect in that case.
        if(_animators == null || _observableValueCollection == null)
        {
            if(_animators == null)                  {Debug.Log("animators list is null!");}
            if(_observableValueCollection == null)  {Debug.Log("observable value collection reference is null!");}
            Debug.Log("Animations loaded incorrectly. Animations for GameObject " + gameObject.GetInstanceID() + " will not run correctly.");
        } 
        else if(_animators.Count==0)
        {
            Debug.Log("Warning. No Animator connected to AnimatorHandler on " + gameObject.name + ". Animations will not run.");
        }     
        else 
        {   
            // Init value added events. 
            _observableValueCollection.ObservableIntAddedEvent += HandleIntAddedEvent;
            _observableValueCollection.ObservableFloatAddedEvent += HandleFloatAddedEvent;
            _observableValueCollection.ObservableBoolAddedEvent += HandleBoolAddedEvent;
            // Init values to listen to Dictionary. (Parameters are assumed to be static and therefore need not be updated.) 
            foreach(Animator a in _animators)
            {
                _parametersToListenToDictionary.Add(a,GatherParametersToListenTo(a));
            }
            //Init value update events.
            foreach(observable_value<int> item in _observableValueCollection.GetObservableIntArray())
            {
                item.UpdateValue += HandleIntUpdateEvent;
            }
            foreach(observable_value<float> item in _observableValueCollection.GetObservableFloatArray())
            {
                item.UpdateValue += HandleFloatUpdateEvent;
            }
            foreach(observable_value<bool> item in _observableValueCollection.GetObservableBoolArray())
            {
                item.UpdateValue += HandleBoolUpdateEvent;
            }
        }
    }
    /// <summary>
    /// Helper method for gathering list of parameters from an Animator.
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    private string[] GatherParametersToListenTo(Animator a)
    {
        List<string> parametersToListenTo = new List<string>();
        foreach (AnimatorControllerParameter param in a.parameters)
        {
            parametersToListenTo.Add(param.name);
        }
        return parametersToListenTo.Distinct().ToArray();
    }
    private void UpdateParametersToListenTo()
    {
        foreach(Animator a in _animators)
        {
            _parametersToListenToDictionary.Add(a,GatherParametersToListenTo(a));
        }
    }

    // Event handlers
    //---------------------------------------------------------------------------------------------
    private void HandleIntUpdateEvent(observable_value<int> context)
    {
        foreach(Animator a in _animators)
        {
            if(AnimatorContainsParameter(a,context.Name))
            {
                a.SetInteger(context.Name, context.Value);
            }
        }
    }
    private void HandleFloatUpdateEvent(observable_value<float> context)
    {
        foreach(Animator a in _animators)
        {
            if(AnimatorContainsParameter(a,context.Name))
            {
                a.SetFloat(context.Name, context.Value); 
            }
        }
    }
    private void HandleBoolUpdateEvent(observable_value<bool> context)
    {
        foreach(Animator a in _animators)
        {
            if(AnimatorContainsParameter(a,context.Name))
            {
                a.SetBool(context.Name, context.Value);
            }
        }
    }
    private void HandleIntAddedEvent(observable_value<int> observableValue)
    {
        foreach(Animator a in _animators)
        {
            if(AnimatorContainsParameter(a,observableValue.Name))
            {
                observableValue.UpdateValue += HandleIntUpdateEvent;
            }
        }
    }
    private void HandleFloatAddedEvent(observable_value<float> observableValue)
    {
        foreach(Animator a in _animators)
        {
            if(AnimatorContainsParameter(a,observableValue.Name))
            {
                observableValue.UpdateValue += HandleFloatUpdateEvent;
            }
        }
    }
    private void HandleBoolAddedEvent(observable_value<bool> observableValue)
    {
        foreach(Animator a in _animators)
        {
            if(AnimatorContainsParameter(a,observableValue.Name))
            {
                observableValue.UpdateValue += HandleBoolUpdateEvent;
            }
        }
    }
    //---------------------------------------------------------------------------------------------

    /// <summary>
    /// Helper method for determining if an animator contains a parameter using the parameter dictionary.
    /// </summary>
    private bool AnimatorContainsParameter(Animator a, string name)
    {
        try
        {
            return _parametersToListenToDictionary[a].Contains(name);
        } 
        catch(Exception e)
        {
            Debug.Log("Warning. Animator '" + a.name + "' is not a member of _parametersToListenToDictionary. " + 
            "Parameters may not update correctly.");
            Debug.Log(e);
            return false;
        }
    }
}
