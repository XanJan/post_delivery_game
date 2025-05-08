using UnityEngine;
[CreateAssetMenu(fileName = "NewPowerup", menuName = "Powerup/PowerupItem")]
public class speed_powerup : powerup
{
    public override void ApplyPowerup(GameObject target)
    {
        // Apply the speed boost to the player
        //player.ApplySpeedBoost(5, duration);
        Debug.Log("Hej");
    }
}
