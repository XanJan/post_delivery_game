using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class player_instance_manager : singleton_persistent<player_instance_manager>
{
    [SerializeField] private string _interactButtonValueName = "interactButton";
    [SerializeField] private string _playerIdValueName = "playerId";
    [SerializeField] private string _interactButtonKeyboard = "E";
    [SerializeField] private string _interactButtonGamepad = "Y";
    private List<GameObject> _playerInstances= new List<GameObject>();
    private Vector3 _nextSpawnPoint = new Vector3(0,0,0);
    private player_instance_manager(){}
    public void AddPlayerInstance(PlayerInput inp)
    {
        // Keep track of players
        _playerInstances.Add(inp.gameObject);
        // Set interact button if possible
        if(inp.TryGetComponent<observable_value_collection>(out var obvc))
        {
            obvc.AddObservableString(_interactButtonValueName); // Ensure value is present
            obvc.AddObservableInt(_playerIdValueName);
            string s;
            switch(inp.currentControlScheme.ToString())
            {
                case "Gamepad":s = _interactButtonGamepad; // If gamepad, set value
                break;
                default:s = _interactButtonKeyboard; // Default: set to keyboard value
                break;
            }
            obvc.InvokeString(_interactButtonValueName, s);
            obvc.InvokeInt(_playerIdValueName, _playerInstances.Count);
        } 

        // Keep player between scenes
        DontDestroyOnLoad(inp.gameObject);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    public void OnSceneLoad(Scene s, LoadSceneMode m)
    {
        float f = 0;
        foreach(GameObject p in _playerInstances)
        {
            f+=0.8f;
            Vector3 mySpawnPoint = _nextSpawnPoint + new Vector3(f,0,0); // x offset relative to f so all players don't spawn in the same pos
            p.transform.position = mySpawnPoint;
        }
    }
    public void SetNextSpawnPoint(Vector3 v)
    {
        _nextSpawnPoint = v;
    }
}
