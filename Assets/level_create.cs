using System.Collections.Generic;
using UnityEngine;

public class level_create : MonoBehaviour
{
    [SerializeField]
    private GameObject[] neighborhoods;
    private int neighborhoodOrder = 1;
    private List<GameObject> instantiateNeighborhoods = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddNeighborhoodToLevel();
        game_events.current.onEndLevelEnter += CreateNewNeighborhood;
    }

    private void CreateNewNeighborhood()
    {
        RemoveNeighborhoodFromLevel(instantiateNeighborhoods[neighborhoodOrder]);
        AddNeighborhoodToLevel();

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void AddNeighborhoodToLevel()
    {
        GameObject neigh = Instantiate(neighborhoods[neighborhoodOrder % neighborhoods.Length], new Vector3(0, 0, 38f * neighborhoodOrder), Quaternion.identity);
        Debug.Log(neigh);
        instantiateNeighborhoods.Add(neigh);
        neighborhoodOrder++;

    }

    private void RemoveNeighborhoodFromLevel(GameObject stage)
    { 
        stage.gameObject.SetActive(false);
    }
}
