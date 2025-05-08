using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class player_instance_manager : singleton_persistent<player_instance_manager>
{
    [SerializeField] private string _interactButtonValueName = "interactButton";
    [SerializeField] private string _playerIdValueName = "playerId";
    [SerializeField] private float _moveSpeedBase = 5;
    [SerializeField] private string _playerMoveSpeedBaseValueName = "moveSpeedBase";
    [SerializeField] private string _playerMoveSpeedMultiplierPickupValueName = "moveSpeedMultiplierPickup";
    [SerializeField] private string _playerMoveSpeedMultiplierEnvironmentValueName = "moveSpeedMultiplierEnvironment";
    [SerializeField] private string _playerMoveSpeedMultiplierOtherValueName = "moveSpeedMultiplierOther";
    [SerializeField] private string _playerNameValueName = "playerName";
    [SerializeField] private string _interactButtonKeyboard = "E";
    [SerializeField] private string _interactButtonGamepad = "X";
    [SerializeField] private observable_value_collection _obvc;
    private List<GameObject> _playerInstances= new List<GameObject>();
    private player_instance_manager(){}
    public void AddPlayerInstance(PlayerInput inp)
    {
        // Keep track of players
        _playerInstances.Add(inp.gameObject);
        _obvc.InvokeInt("numberOfPlayers",_playerInstances.Count);
        if(GameObject.FindWithTag("Respawn")!=null){inp.transform.position = GameObject.FindWithTag("Respawn").transform.position + new Vector3(0.8f * _playerInstances.Count,0,0);}
        else{inp.transform.position = new Vector3(0.8f * _playerInstances.Count,0,0);}
        // Set interact button if possible
        if(inp.TryGetComponent<observable_value_collection>(out var obvc))
        {
            obvc.AddObservableString(_interactButtonValueName); // Ensure value is present
            obvc.AddObservableInt(_playerIdValueName);
            obvc.AddObservableFloat(_playerMoveSpeedBaseValueName);
            obvc.AddObservableFloat(_playerMoveSpeedMultiplierPickupValueName);
            obvc.AddObservableFloat(_playerMoveSpeedMultiplierEnvironmentValueName);
            obvc.AddObservableFloat(_playerMoveSpeedMultiplierOtherValueName);
            obvc.AddObservableString(_playerNameValueName);
            string s;
            switch(inp.currentControlScheme.ToString())
            {
                case "Gamepad":s = _interactButtonGamepad; // If gamepad, set value
                break;
                default:s = _interactButtonKeyboard; // Default: set to keyboard value
                break;
            }
            obvc.InvokeString(_interactButtonValueName, s);
            obvc.InvokeFloat(_playerMoveSpeedBaseValueName,_moveSpeedBase);
            obvc.InvokeFloat(_playerMoveSpeedMultiplierPickupValueName,1);
            obvc.InvokeFloat(_playerMoveSpeedMultiplierEnvironmentValueName,1);
            obvc.InvokeFloat(_playerMoveSpeedMultiplierOtherValueName,1);
            obvc.InvokeInt(_playerIdValueName, _playerInstances.Count);
            obvc.InvokeString(_playerNameValueName, "Paul");
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
            p.transform.position = GameObject.FindWithTag("Respawn").transform.position + new Vector3(f,0,0);
        }
    }
}
