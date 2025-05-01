using UnityEngine;

public class mimic_transform : MonoBehaviour
{
    [SerializeField] Transform other;

    // Update is called once per frame
    void Update()
    {
        if(other!=null) other.transform.position = transform.position;
    }
}
