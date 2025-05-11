using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
/// <summary>
/// Keeps track of controller instances. Set the controller position to spawn location on scene load
/// A spawn location is added to a scene by adding a gameobject with the "Respawn" tag in scene root.
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
    public void OnPlayerJoinedHandler(PlayerInput inp)
    {
        // Keep track of players
        _controllerInstances.Add(inp.gameObject);
        _obvc.InvokeInt("numberOfPlayers",_controllerInstances.Count);
        // Apply offset to players when spawning so they dont occupy the same space
        GameObject temp;
        inp.transform.position = (temp = GameObject.FindWithTag("Respawn"))==null? new Vector3(_controllerInstances.Count * 0.8f,0,0) : 
                temp.transform.position + new Vector3(_controllerInstances.Count*0.8f,0,0);  
    }

    public void OnPlayerLeaveHandler(PlayerInput inp)
    {
        _controllerInstances.Remove(inp.gameObject);
        _obvc.InvokeInt("numberOfPlayers",_controllerInstances.Count);
    }

    public bool TryGetControllerSpawnPos(controller_input controller, out Vector3 spawnPos)
    {
        spawnPos = GameObject.FindWithTag("Respawn")==null?Vector3.zero:GameObject.FindWithTag("Respawn").transform.position;
        for(int i = 0 ; i < _controllerInstances.Count ; i++)
        {
            if(controller.gameObject == _controllerInstances[i]) {spawnPos += new Vector3(1f*i,0,0); return true;}
        }
        
        if(spawnPos!=null){spawnPos += new Vector3(1f*_controllerInstances.Count,0,0); return true;}
        else return false;
    }
    public void SwitchCurrentActionMap(string s)
    {
        foreach(GameObject go in _controllerInstances)
        {
            if(go.TryGetComponent<PlayerInput>(out var playerInput))
            {
                playerInput.SwitchCurrentActionMap(s);
            }
        }
    }
}
