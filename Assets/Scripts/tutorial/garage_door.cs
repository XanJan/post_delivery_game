using Unity.VisualScripting;
using UnityEngine;

public class garage_door : MonoBehaviour
{
    private bool _move = false;
    private float _secondsBetween = 0.25f;
    private float _secondsSinceLast = 0;
    void FixedUpdate()
    {
        
        if(_move && !(transform.position.y >= 4.5f))
        {
            _secondsSinceLast += Time.deltaTime;
            if(_secondsSinceLast >= _secondsBetween)
            {
                transform.position += Vector3.up * 0.1f;
                _secondsSinceLast = 0;
            }
            
        }
    }
    public void StartMoving()
    {
        _move = true;
    }

    public void StopMoving()
    {
        _move = false;
    }
}
