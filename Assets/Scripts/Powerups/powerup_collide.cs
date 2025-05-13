using UnityEngine;

public class powerup_collide : MonoBehaviour
{
    public powerup pu;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            pu.ApplyPowerup(other.gameObject);
            SoundManager.PlaySound(SoundType.PowerupPickup);
            Destroy(this.gameObject);
        }
    }
}
