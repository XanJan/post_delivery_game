using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInputManager))]
public class player_prefab_switcher : MonoBehaviour
{
    private PlayerInputManager _inputManager;
    [SerializeField] List<GameObject> _playerPrefabs;
    int i = 0;

    void Awake()
    {
        _inputManager = GetComponent<PlayerInputManager>();
        if(_inputManager.playerPrefab==null && _playerPrefabs.Count > 0)
        {
            _inputManager.playerPrefab = _playerPrefabs[0];
        }
        if(_playerPrefabs.Count==0)
        {
            Debug.Log("Warning: Player prefab switcher is not initialized with any player prefabs to switch to. Players will join with the same prefab. On gameObject " + gameObject.name + ".");
        }
    }
    public void NextPrefab()
    {
        if(_playerPrefabs.Count>0)
        {
            i++;
            if(i== _playerPrefabs.Count)
            {
                i = 0;
            }
            _inputManager.playerPrefab = _playerPrefabs[i];
        } else{Debug.Log("Warning: Player prefab switcher could not switch prefab. On gameObject " + gameObject.name + ".");}
    }
}
