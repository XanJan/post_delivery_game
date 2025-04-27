using System.Collections.Generic;
using UnityEngine;

public class level_create : MonoBehaviour
{
    [SerializeField]
    private GameObject[] neighborhoods;
    private int neighborhoodOrder = 1;
    private List<GameObject> instantiateNeighborhoods = new List<GameObject>();
    private bool isNeighborhoodFinished = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddNeighborhoodToLevel();
        game_events.current.onEndLevelEnter += CreateNewNeighborhood;
        game_events.current.onNeighborhoodFinished += NeighborhoodFinished;
    }

    private void NeighborhoodFinished()
    {
        isNeighborhoodFinished = true;
    }
    private void CreateNewNeighborhood()
    {
        if(isNeighborhoodFinished)
        {
            //Debug.Log(instantiateNeighborhoods.Count);
            AddNeighborhoodToLevel();
            RemoveNeighborhoodFromLevel(instantiateNeighborhoods[0]);
            isNeighborhoodFinished = false;
        }

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
        if(instantiateNeighborhoods.Count >= 4)
        {
            stage.gameObject.SetActive(false);
            instantiateNeighborhoods.RemoveAt(0);
        }
    }
}
