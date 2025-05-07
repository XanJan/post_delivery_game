using UnityEngine;
[CreateAssetMenu(fileName = "NewPowerup", menuName = "Powerup/PowerupItem")]
public abstract class powerup : ScriptableObject
{
    public string powerUpName;
    public string description;
    public float duration;

    public abstract void ApplyPower();

}
