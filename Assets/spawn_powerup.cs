using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class spawn_powerup : MonoBehaviour
{
    [SerializeField] private List<GameObject> powerups;

    private void Start()
    {
        StartCoroutine(SpawnPowerup(Random.Range(10f, 20f)));
    }

    private IEnumerator SpawnPowerup(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            Instantiate(powerups[Random.Range(0, powerups.Count)], new Vector3(this.transform.position.x + Random.Range(-8f, 8f), this.transform.position.y, this.transform.position.z + Random.Range(-18f, 18f)), Quaternion.identity);
            //StartCoroutine(SpawnPowerup(Random.Range(1f, 10f)));
        }
    }

}
