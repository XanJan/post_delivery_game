using UnityEngine;
/// <summary>
/// Handles initilizations of values in the player observable value collection.
/// </summary>
public class player_initializer : MonoBehaviour
{
    [SerializeField] observable_value_collection _obvc;
    [SerializeField] float _moveSpeedBase=5f;
    public string PlayerName="Paul";
    void Awake()
    {
        if(_obvc==null && TryGetComponent<observable_value_collection>(out var res)) _obvc = res;
    }
    void Start()
    {
        if(_obvc!=null)
        {
            _obvc.InvokeFloat("moveSpeedBase",_moveSpeedBase);
            _obvc.InvokeFloat("moveSpeedMultiplierPickup",1);
            _obvc.InvokeFloat("moveSpeedMultiplierEnvironment",1);
            _obvc.InvokeFloat("moveSpeedMultiplierOther",1);
            _obvc.InvokeString("playerName", PlayerName);
        }
        else{Debug.Log("Warning: observable value collection not found in player_initializer. Player may not be initialized with correct values.");}
    }
}
