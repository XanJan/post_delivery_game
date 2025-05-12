using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
[CreateAssetMenu(fileName = "NewPowerup", menuName = "Powerup/Speed Powerup")]
public class speed_powerup : powerup
{
    public float speedMultiplier;
    public float time;
    public override void ApplyPowerup(GameObject target)
    {
        player_movement script = target.GetComponent<player_movement>();
        script.StartCoroutine(TemporarySpeedBoost(script));

    }
    private IEnumerator TemporarySpeedBoost(player_movement target)
    {
        //obvc.InvokeFloat("moveSpeedMultiplierOther", NEWVALUE); 
        target.GetComponent<observable_value_collection>().InvokeFloat("moveSpeedMultiplierOther", speedMultiplier);
        //target.OnUpdateMoveSpeedBase(speedMultiplier);
        yield return new WaitForSeconds(time);
        target.GetComponent<observable_value_collection>().InvokeFloat("moveSpeedMultiplierOther", 5f);
        //target.OnUpdateMoveSpeedBase(5f); // Reset to normal speed
    }
}
