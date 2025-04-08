using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class player_instance_manager : singleton_persistent<player_instance_manager>
{
    private List<GameObject> _playerInstances;
    private Vector3 _nextSpawnPoint = new Vector3(0,0,0);
    private player_instance_manager(){}
    public void AddPlayerInstance(PlayerInput inp)
    {
        // Keep track of players
        _playerInstances.Add(inp.gameObject);
        // Keep player between scenes
        DontDestroyOnLoad(inp.gameObject);
    }

    new void Awake()
    {
        base.Awake();
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
