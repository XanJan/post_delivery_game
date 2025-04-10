using UnityEngine;

public class conveyor_belt : MonoBehaviour
{
    [SerializeField] private float speed = 2f;



    void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = transform.forward * speed;
        }
    }
}
