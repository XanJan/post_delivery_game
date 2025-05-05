using UnityEngine;

public class speed_powerup : powerup
{
    public override void ApplyPowerup()
    {
        // Apply the speed boost to the player
        player.ApplySpeedBoost(5, duration);
    }
}
