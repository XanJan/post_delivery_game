using System.Collections;
using System.Collections.Generic;
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
        target.OnUpdateMoveSpeedBase("", speedMultiplier);
        yield return new WaitForSeconds(time);
        target.OnUpdateMoveSpeedBase("", 5f); // Reset to normal speed
    }
}
