using UnityEngine;

public class singleton_persistent<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get{
            if(_instance==null)
            {
                GameObject go = new GameObject();
                go.name = typeof(T).Name;
                go.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<T>();
            }
            return _instance;
        }
    }
    public void Awake()
    {
        if(_instance==null)
        {
            _instance = this as T;
            Debug.Log("TRY");
            DontDestroyOnLoad(this);
            Debug.Log("SUCCESS");
        } else { Destroy(this); }
    }
}
