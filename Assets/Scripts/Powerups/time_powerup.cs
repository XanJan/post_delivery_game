using UnityEngine;
[CreateAssetMenu(fileName = "NewPowerup", menuName = "Powerup/Time Powerup")]
public class time_powerup : powerup
{
    public float amount;
    public override void ApplyPowerup(GameObject target)
    {
        GameObject.FindGameObjectWithTag("Timer").GetComponent<timer>().AddTime(amount);
    }
}
