using UnityEngine;

public class end_trigger_area : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        game_events.current.EndLevelEnter();
    }
}
