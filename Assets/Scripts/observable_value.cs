//----------------------------------------------------------------------------
// Authors : Alexander Lisborg
//----------------------------------------------------------------------------

/// <summary>
/// Represents a generic value with an attached event that trigger on value update.
/// NOT THREAD SAFE.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public class observable_value<T>
{
    /// <summary>
    /// The name of the value as a string representation. This is used since Unity
    /// often work with strings as identifiers of values. This name is given in
    /// the constructor and can't be changed.
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// The current value. Not thread safe!
    /// </summary>
    public T Value {get;private set;}
    /// <summary>
    /// Delegate type defenition. Name is passed with the event value for identification.
    /// </summary>
    /// <param name="name">String name of value, used for identification.</param>
    /// <param name="v">New value.</param>
    public delegate void UpdateValueDelegate(string name, T v);
    /// <summary>
    /// Triggers whenever value gets updated. (Even if it is unchanged!)
    /// </summary>
    public event UpdateValueDelegate UpdateValue = default;
    /// <summary>
    /// Only constructor, name defenition needed when initializing.
    /// </summary>
    /// <param name="name"></param>
    public observable_value(string name)
    {
        Name = name;
    }
    /// <summary>
    /// Setter for Value that trigger an UpdateValue event.
    /// </summary>
    /// <param name="v"></param>
    public void InvokeEvent(T v)
    {
        UpdateValue?.Invoke(Name, v);
        Value = v;
    }    
}
