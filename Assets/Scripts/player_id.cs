using UnityEngine;

public class player_id : MonoBehaviour
{
    [SerializeField] private observable_value_collection _obvc;
    [SerializeField] private string _playerIdValueName = "playerId";
    [SerializeField] private int _id;
    void Start()
    {
        if(_obvc!=null)
        {
            _obvc.InvokeInt(_playerIdValueName,_id);
        }
    }
}
