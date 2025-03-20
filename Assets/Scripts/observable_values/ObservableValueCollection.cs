//----------------------------------------------------------------------------
// Authors : Alexander Lisborg
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Collection of observable values. Used for passing values between Unity scripts in an
/// event based, modular manner. Attach to a gameobject to handle variables "belonging" to
/// that gameobject. Names are used as identifiers and should match corresponding
/// names in other scripts. Duplicate names are discarded. NOT THREAD SAFE.
/// </summary>
public class ObservableValueCollection : MonoBehaviour
{
    // Lists of value names are used to configure the value collection from within Unity.
    // for each name specified, an ObservableValue is instantiated and added to the
    // Dictionary corresponding to its type. Duplicates are discarded.
    [Header("Observable Value Names... NO DUPLICATES")]
    [SerializeField] private List<string> _intNames;
    [SerializeField] private List<string> _floatNames;
    [SerializeField] private List<string> _boolNames;
    [SerializeField] private List<string> _vector2Names;

    // List of all the names of the currently active observable values.
    private List<string> _names;
    /// <summary>
    /// Dictionary of observable ints. Use ObservableInts[name] to get a value. Don't
    /// forget to handle exceptions when getting.
    /// </summary>
    private Dictionary<string,ObservableValue<int>> _observableInts;
    /// <summary>
    /// Dictionary of observable floats. Use ObservableFloats[name] to get a value. Don't
    /// forget to handle exceptions when getting.
    /// </summary>
    private Dictionary<string,ObservableValue<float>> _observableFloats;
    /// <summary>
    /// Dictionary of observable bools. Use ObservableBools[name] to get a value. Don't
    /// forget to handle exceptions when getting.
    /// </summary>
    private Dictionary<string,ObservableValue<bool>> _observableBools;
    /// <summary>
    /// Dictionary of observable Vector2s. Use ObservableVector2s[name] to get a value. Don't
    /// forget to handle exceptions when getting.
    /// </summary>
    private Dictionary<string,ObservableValue<Vector2>> _observableVector2s;
    /// <summary>
    /// NOT TYPE SAFE, can be used to store ObservableValues without specifying type
    /// and relying on usafe casts. Should be avoided in most cases.
    /// </summary>
    private Dictionary<string,ObservableValue<object>> _observableObjects;
    // Delegates
    //----------------------------------------------------------------------------
    public delegate void DelegateObservableIntAdded(ObservableValue<int> observableInt);
    public delegate void DelegateObservableFloatAdded(ObservableValue<float> observableFloat);
    public delegate void DelegateObservableBoolAdded(ObservableValue<bool> observableBool);
    public delegate void DelegateObservableVector2Added(ObservableValue<Vector2> observableVector2);
    public delegate void DelegateObservableObjectAdded(ObservableValue<object> observableObject);
    //----------------------------------------------------------------------------
    
    //Events
    //----------------------------------------------------------------------------
    /// <summary>
    /// Invoked when int is added to observable value collection.
    /// </summary>
    public event DelegateObservableIntAdded ObservableIntAddedEvent ;
    /// <summary>
    /// Invoked when float is added to observable value collection.
    /// </summary>
    public event DelegateObservableFloatAdded ObservableFloatAddedEvent;
    /// <summary>
    /// Invoked when bool is added to observable value collection.
    /// </summary>
    public event DelegateObservableBoolAdded ObservableBoolAddedEvent ;
    /// <summary>
    /// Invoked when Vector2 is added to observable value collection.
    /// </summary>
    public event DelegateObservableVector2Added ObservableVector2AddedEvent ;
    /// <summary>
    /// Invoked when object is added to observable value collection.
    /// </summary>
    public event DelegateObservableObjectAdded ObservableObjectAddedEvent ;
    //----------------------------------------------------------------------------

    /// <summary>
    /// Instantiates instance fields. Initilization takes place in Awake().
    /// </summary>
    public ObservableValueCollection()
    {
        _observableInts = new Dictionary<string,ObservableValue<int>>();
        _observableFloats = new Dictionary<string,ObservableValue<float>>();
        _observableBools = new Dictionary<string,ObservableValue<bool>>();
        _observableVector2s = new Dictionary<string,ObservableValue<Vector2>>();
        _observableObjects = new Dictionary<string, ObservableValue<object>>();
        _names = new List<string>();
        ObservableIntAddedEvent = default;
        ObservableFloatAddedEvent = default;
        ObservableBoolAddedEvent = default;
        ObservableVector2AddedEvent = default;
        ObservableObjectAddedEvent = default;
    }
    /// <summary>
    /// Since Awake() is called before Start(), all event handlers should be set up in respective
    /// Start() methods. If an event handler is instantiated within an Awake() method, there's
    /// risk of null exceptions for uninitialized.
    /// 
    /// Unity calls Awake on scripts derived from MonoBehaviour in the following scenarios:
    ///     * The parent GameObject is active and initializes on Scene load
    ///     * The parent GameObject goes from inactive to active
    ///     * After initialization of a parent GameObject created with Object.Instantiate
    /// </summary>
    void Awake()
    {
        // Init names list to the Union of all serialized name lists. (throws away duplicate names)
        _names = _names.Union(_intNames).Union(_floatNames).Union(_boolNames).Union(_vector2Names).ToList();
        foreach(string name in _names){Debug.Log("Name : " + name);}
        // Keeps track of unique names so no duplicates are added
        List<string> uniqueNamesCopy = new List<string>(_names); 
        // Add new observable value for each name in each name list.
        //_observableFloats.Add("moveSpeed",new ObservableValue<float>("moveSpeed"));
        foreach(string s in _intNames.Distinct()) 
        {
            if(uniqueNamesCopy.Contains(s))
            {
                uniqueNamesCopy.Remove(s);
                if(!_observableInts.ContainsKey(s))
                {
                    _observableInts.Add(s,new ObservableValue<int>(s));
                }
                
            }
        }
        foreach(string s in _floatNames.Distinct()) 
        {
            if(uniqueNamesCopy.Contains(s))
            {
                uniqueNamesCopy.Remove(s);
                if(!_observableFloats.ContainsKey(s))
                {
                    _observableFloats.Add(s,new ObservableValue<float>(s));
                }
            }
        }
        foreach(string s in _boolNames.Distinct())
        {
            if(uniqueNamesCopy.Contains(s))
            {
                uniqueNamesCopy.Remove(s);
                if(!_observableBools.ContainsKey(s))
                {
                    _observableBools.Add(s,new ObservableValue<bool>(s));
                }
                
            }
        }
        foreach(string s in _boolNames.Distinct())
        {
            if(uniqueNamesCopy.Contains(s))
            {
                uniqueNamesCopy.Remove(s);
                if(!_observableVector2s.ContainsKey(s))
                {
                    _observableVector2s.Add(s,new ObservableValue<Vector2>(s));
                }
            }
        }
    }
    /// <summary>
    /// Add new ObservableValue<int> to the collection. If it already exists, does nothing.
    /// </summary>
    /// <param name="name">Name of the observable int to add.</param>
    public void AddObservableInt(string name)
    {
        if(!_names.Contains(name))
        {
            ObservableValue<int> observableValue = new ObservableValue<int>(name);
            UniqueAdd(_names,name);
            _observableInts.Add(name, observableValue);
            if(ObservableIntAddedEvent != null)
            {
                ObservableIntAddedEvent?.Invoke(observableValue);
            }
        }
    }
    /// <summary>
    /// Add new ObservableValue<float> to the collection. If it already exists, does nothing.
    /// </summary>
    /// <param name="name">Name of the observable float to add.</param>
    public void AddObservableFloat(string name)
    {
        if(!_names.Contains(name))
        {
            ObservableValue<float> observableValue = new ObservableValue<float>(name);
            UniqueAdd(_names,name);
            _observableFloats.Add(name, observableValue);
            ObservableFloatAddedEvent?.Invoke(observableValue);
        }
    }
    /// <summary>
    /// Add new ObservableValue<bool> to the collection. If it already exists, does nothing.
    /// </summary>
    /// <param name="name">Name of the observable bool to add.</param>
    public void AddObservableBool(string name)
    {
        if(!_names.Contains(name))
        {
            ObservableValue<bool> observableValue = new ObservableValue<bool>(name);
            UniqueAdd(_names,name);
            _observableBools.Add(name, observableValue);
            ObservableBoolAddedEvent?.Invoke(observableValue);
        }
    }
    /// <summary>
    /// Add new ObservableValue<Vector2> to the collection. If it already exists, does nothing.
    /// </summary>
    /// <param name="name">Name of the observable Vector2 to add.</param>
    public void AddObservableVector2(string name)
    {
        if(!_names.Contains(name))
        {
            UniqueAdd(_names,name);
            ObservableValue<Vector2> observableValue = new ObservableValue<Vector2>(name);
            _observableVector2s.Add(name, observableValue);
            ObservableVector2AddedEvent?.Invoke(observableValue);
        }
    }
    /// <summary>
    /// NOT TYPE SAFE. Use with care, casting objects is not guaranteed to work. Add new ObservableValue<object> to 
    /// the collection. If it already exists, does nothing.
    /// </summary>
    /// <param name="name">Name of the observable object to add.</param>
    public void AddObservableObject(string name)
    {
        if(!_names.Contains(name))
        {
            UniqueAdd(_names,name);
            ObservableValue<object> observableValue = new ObservableValue<object>(name);
            _observableObjects.Add(name,observableValue);
            ObservableObjectAddedEvent?.Invoke(observableValue);
        }
    }
    /// <summary>
    /// Getter for observable ints. Throws KeyNotFoundException if not found.
    /// </summary>
    /// <param name="name">Name of the observable int.</param>
    /// <returns>Instance of the observable int mathing the provided name if it exists in the observable value collection.</returns>
    public ObservableValue<int> GetObservableInt(string name){try{return _observableInts[name];}catch(Exception){throw;}}
    /// <summary>
    /// Getter for observable floats. Throws KeyNotFoundException if not found.
    /// </summary>
    /// <param name="name">Name of the observable float.</param>
    /// <returns>Instance of the observable float mathing the provided name if it exists in the observable value collection.</returns>
    public ObservableValue<float> GetObservableFloat(string name){try{return _observableFloats[name];}catch(Exception){throw;}}
    /// <summary>
    /// Getter for observable bools. Throws KeyNotFoundException if not found.
    /// </summary>
    /// <param name="name">Name of the observable bool.</param>
    /// <returns>Instance of the observable bool mathing the provided name if it exists in the observable value collection.</returns>
    public ObservableValue<bool> GetObservableBool(string name){try{return _observableBools[name];}catch(Exception){throw;}}
    /// <summary>
    /// Getter for observable Vector2s. Throws KeyNotFoundException if not found.
    /// </summary>
    /// <param name="name">Name of the observable Vector2.</param>
    /// <returns>Instance of the observable Vector2 mathing the provided name if it exists in the observable value collection.</returns>
    public ObservableValue<Vector2> GetObservableVector2(string name){try{return _observableVector2s[name];}catch(Exception){throw;}}
    /// <summary>
    /// NOT TYPE SAFE. Use with care, casting objects is not guaranteed to work. Getter for observable objects. Throws KeyNotFoundException. 
    /// </summary>
    /// <param name="name">Name of the observable object.</param>
    /// <returns>Instance of the observable object mathing the provided name if it exists in the observable value collection.</returns>
    public ObservableValue<object> GetObservableObject(string name){try{return _observableObjects[name];}catch(Exception){throw;}}
    /// <summary>
    /// Helper method for adding only if the item does not already exist in a list.
    /// </summary>
    private void UniqueAdd<T>(List<T> list, T item){if(!list.Contains(item)){list.Add(item);}}
    /// <summary>
    /// Remove observable value from the collection.
    /// </summary>
    /// <param name="name">Name of the observable value to remove.</param>
    public void Remove(string name)
    {
        _names.Remove(name);
        _intNames.Remove(name);
        _floatNames.Remove(name);
        _boolNames.Remove(name);
        _vector2Names.Remove(name);
        _observableInts.Remove(name);
        _observableFloats.Remove(name);
        _observableBools.Remove(name);
        _observableVector2s.Remove(name);
        _observableObjects.Remove(name);
    }
    /// <summary>
    /// Update in the collection. Invokes UpdateValue event on the ObservableValue.
    /// </summary>
    /// <param name="name">Name of the observable value to update.</param>
    /// <param name="i">New value.</param>
    public void InvokeInt(string name, int i){GetObservableInt(name).InvokeEvent(i);}
    /// <summary>
    /// Update in the collection. Invokes UpdateValue event on the ObservableValue.
    /// </summary>
    /// <param name="name">Name of the observable value to update.</param>
    /// <param name="i">New value.</param>
    public void InvokeFloat(string name, float f){GetObservableFloat(name).InvokeEvent(f);}
    /// <summary>
    /// Update in the collection. Invokes UpdateValue event on the ObservableValue.
    /// </summary>
    /// <param name="name">Name of the observable value to update.</param>
    /// <param name="i">New value.</param>
    public void InvokeBool(string name, bool b){GetObservableBool(name).InvokeEvent(b);}
    /// <summary>
    /// Update in the collection. Invokes UpdateValue event on the ObservableValue.
    /// </summary>
    /// <param name="name">Name of the observable value to update.</param>
    /// <param name="i">New value.</param>
    public void InvokeVector2(string name, Vector2 v2){GetObservableVector2(name).InvokeEvent(v2);}
    /// <summary>
    /// NOT TYPE SAFE. Use with care, casting objects is not guaranteed to work.
    /// Update in the collection. Invokes UpdateValue event on the ObservableValue.
    /// </summary>
    /// <param name="name">Name of the observable value to update.</param>
    /// <param name="i">New value.</param>
    public void InvokeObject(string name, object o){GetObservableObject(name).InvokeEvent(o);}
    /// <summary>
    /// Get by copy.
    /// </summary>
    /// <returns>All observable ints in the collection.</returns>
    public ObservableValue<int>[] GetObservableIntArray(){return _observableInts.Values.ToArray();}
    /// <summary>
    /// Get by copy.
    /// </summary>
    /// <returns>All observable floats in the collection.</returns>
    public ObservableValue<float>[] GetObservableFloatArray(){return _observableFloats.Values.ToArray();}
    /// <summary>
    /// Get by copy.
    /// </summary>
    /// <returns>All observable bools in the collection.</returns>
    public ObservableValue<bool>[] GetObservableBoolArray(){return _observableBools.Values.ToArray();}
    /// <summary>
    /// Get by copy.
    /// </summary>
    /// <returns>All observable Vector2s in the collection.</returns>
    public ObservableValue<Vector2>[] GetObservableVector2Array(){return _observableVector2s.Values.ToArray();}
    /// <summary>
    /// NOT TYPE SAFE. Use with care, casting objects is not guaranteed to work. Get by copy.
    /// </summary>
    /// <returns>All observable objects in the collection.</returns>
    public ObservableValue<object>[] GetObservableObjectsArray(){return _observableObjects.Values.ToArray();}

    /// <summary>
    /// Check if an observable value is present in the collection.
    /// </summary>
    /// <param name="name">Name of the observable value to check for.</param>
    public bool Contains(string name){return _names.Contains(name);}
}
