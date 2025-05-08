using UnityEngine;
[CreateAssetMenu(fileName = "NewPowerup", menuName = "Powerup/Speed Powerup")]
public class speed_powerup : powerup
{
    public float speedMultiplier;
    public override void ApplyPowerup(GameObject target)
    {
        target.GetComponent<player_movement>().OnUpdateMoveSpeedMultiplierPickup("", speedMultiplier);
        
    }
}
