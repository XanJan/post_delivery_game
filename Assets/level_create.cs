using UnityEngine;

public class level_create : MonoBehaviour
{
    [SerializeField]
    private GameObject[] neighborhoods;
    private int neighborhoodOrder = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        AddNeighborhoodToLevel();
    }
    private void AddNeighborhoodToLevel()
    {
        Instantiate(neighborhoods[neighborhoodOrder % neighborhoods.Length], new Vector3(0, 0, 38f * neighborhoodOrder), Quaternion.identity);
        neighborhoodOrder++;
    }

    private void RemoveNeighborhoodFromLevel(GameObject stage)
    { 
        stage.gameObject.SetActive(false);
    }
}
