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
        
        // Assign a unique neighborhood ID to created neighborhood
        neighborhood_manager neighManager = neigh.GetComponent<neighborhood_manager>();
        if (neighManager == null)
        {
            neighManager = neigh.AddComponent<neighborhood_manager>();
        }
        
        // Set the neighborhood ID via reflection to access the private field
        var field = typeof(neighborhood_manager).GetField("neighborhoodId", 
            System.Reflection.BindingFlags.Instance | 
            System.Reflection.BindingFlags.NonPublic);
            
        if (field != null)
        {
            field.SetValue(neighManager, neighborhoodOrder);
            Debug.Log($"Assigned ID {neighborhoodOrder} to new neighborhood");
        }
        
        Debug.Log(neigh);
        instantiateNeighborhoods.Add(neigh);
        neighborhoodOrder++;
    }

    private void RemoveNeighborhoodFromLevel(GameObject stage)
    { 
        if(instantiateNeighborhoods.Count >= 5)
        {
            stage.gameObject.SetActive(false);
            instantiateNeighborhoods.RemoveAt(0);
        }
    }
}
