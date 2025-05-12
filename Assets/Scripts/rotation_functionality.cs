//----------------------------------------------------------------------------
// Authors : Alexander Lisborg
//----------------------------------------------------------------------------
using UnityEngine;

/// <summary>
/// Simple script for rotating a gameobject according to a value in an 
/// observable value collection. Works without polling. Prints debug
/// messages and does nothing if serialize fields are not instantiated 
/// properly.
/// </summary>
public class rotation_functionality : MonoBehaviour
{
    [SerializeField] private observable_value_collection _observableValueCollection;
    [SerializeField] private Transform _toRotate;
    [SerializeField] private string _vector2Name;
    // Part of debug messages.
    private string NullMessage {get {return ("is null in rotation_functionality on gameObject " 
                                                + gameObject.name + 
                                                    ". Rotations will not function properly.");}}
    // Value update event is subscribed to in start. Also print debug warnings if needed.
    void Start()
    {
        if(_toRotate==null){Debug.Log("Warning: To Rotate transform " + NullMessage);}
        if(_vector2Name==null){Debug.Log("Warning: Vector2 Name " + NullMessage);}
        if(_observableValueCollection!=null)
        {
            // Ensure feet direction vector2 is in the collection
            _observableValueCollection.AddObservableVector2(_vector2Name);
            _observableValueCollection.GetObservableVector2(_vector2Name).UpdateValue += HandleValueUpdate;
        } else {Debug.Log("Warning: Observable value collection " + NullMessage);}
    }

    private void HandleValueUpdate(observable_value<Vector2> context)
    {
        if(_toRotate!=null){_toRotate.rotation = Quaternion.LookRotation(context.Value);}
    }
}
