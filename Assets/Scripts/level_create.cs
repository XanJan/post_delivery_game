using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class level_create : MonoBehaviour
{
    public static level_create Instance {get; private set;}
    [SerializeField]
    private GameObject[] neighborhoods;
    private int neighborhoodOrder = 1;
    private List<GameObject> instantiateNeighborhoods = new List<GameObject>();
    private bool isNeighborhoodFinished = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject startingNeighborhood;

    void Awake()
    {
        Instance = this;
        if (startingNeighborhood != null){
            instantiateNeighborhoods.Add(startingNeighborhood);
        }
    }
    void Start()
    {
        game_events.current.RaiseNeighborhoodGenerated();
        game_events.current.onEndLevelEnter += CreateNewNeighborhood;
        game_events.current.onNeighborhoodFinished += NeighborhoodFinished;
        AddNeighborhoodToLevel();
        game_events.current.RaiseNeighborhoodGenerated();
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
            game_events.current.RaiseNeighborhoodGenerated();
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

    public int GetTotalDeliveryZones(){
        int totalZones = 0;
        foreach(GameObject neighborhood in instantiateNeighborhoods){
            neighborhood_manager manager = neighborhood.GetComponent<neighborhood_manager>();
            if(manager != null){
                totalZones += neighborhood.transform.childCount;
            }
        }
        return totalZones;
    }

    public int GetTotalPackagesToDeliver(){
        int total = 0;
        foreach(var neigh in instantiateNeighborhoods){
            var zones = neigh.GetComponentsInChildren<delivery_zone>();
            foreach(var zone in zones){
                total += zone.maxPackages - zone.GetCurrentPackages();
            }
        }
        return total;
    }
}
