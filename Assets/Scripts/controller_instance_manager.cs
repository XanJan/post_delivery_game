using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
/// <summary>
/// Keeps track of controller instances. Set the controller position to spawn location on scene load
/// A spawn location is added to a scene by adding a gameobject named "Spawn" in scene root.
/// </summary>
[RequireComponent(typeof(PlayerInputManager))]
[RequireComponent(typeof(observable_value_collection))]
public class controller_instance_manager : singleton_persistent<controller_instance_manager>
{
    [SerializeField] private observable_value_collection _obvc;
    [SerializeField] private PlayerInputManager _playerInputManager;
    private List<GameObject> _controllerInstances= new List<GameObject>();
    private controller_instance_manager(){}
    /// <summary>
    /// Event handler for when players join with a new controller.
    /// </summary>
    /// <param name="inp">Joined PlayerInput</param>
    private void OnPlayerJoinedHandler(PlayerInput inp)
    {
        // Keep track of players
        _controllerInstances.Add(inp.gameObject);
        _obvc.InvokeInt("numberOfPlayers",_controllerInstances.Count);
        // Apply offset to players when spawning so they dont occupy the same space
        GameObject temp;
        inp.transform.position = (temp = GameObject.FindWithTag("Spawn"))==null? new Vector3(_controllerInstances.Count * 0.8f,0,0) : 
                temp.transform.position + new Vector3(_controllerInstances.Count*0.8f,0,0);  
    }

    new void Awake()
    {
        base.Awake();
        if(_playerInputManager==null&&TryGetComponent<PlayerInputManager>(out var res)) _playerInputManager = res;
        if(_playerInputManager==null&&TryGetComponent<observable_value_collection>(out var res2)) _obvc = res2;
    }
    void OnEnable()
    {
        _playerInputManager.onPlayerJoined+=OnPlayerJoinedHandler;
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    void OnDisable()
    {
        _playerInputManager.onPlayerJoined-=OnPlayerJoinedHandler;
        SceneManager.sceneLoaded -= OnSceneLoad;    
    }

    public void OnSceneLoad(Scene s, LoadSceneMode m)
    {
        GameObject temp;
        Vector3 spawnPos = (temp=GameObject.FindWithTag("Spawn"))==null? Vector3.zero : temp.transform.position;
        float f = 0;
        foreach(GameObject p in _controllerInstances)
        {
            f+=0.8f;
            p.transform.position = spawnPos + new Vector3(f,0,0);
        }
    }
}
