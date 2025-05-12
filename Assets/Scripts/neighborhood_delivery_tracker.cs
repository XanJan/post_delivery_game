using System.Collections.Generic;
using UnityEngine;

// This class keeps track of all neighborhoods and their completion status
public class neighborhood_delivery_tracker : MonoBehaviour
{
    private static neighborhood_delivery_tracker _instance;
    public static neighborhood_delivery_tracker Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<neighborhood_delivery_tracker>();
                if (_instance == null)
                {
                    GameObject trackerObject = new GameObject("NeighborhoodDeliveryTracker");
                    _instance = trackerObject.AddComponent<neighborhood_delivery_tracker>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            return _instance;
        }
    }

    // Dictionary to track neighborhood completion status
    private Dictionary<int, bool> neighborhoodCompletionStatus = new Dictionary<int, bool>();

    void Awake()
    {
        // Ensure we have only one instance (Singleton pattern)
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Register new neighborhood
    public void RegisterNeighborhood(int neighborhoodId)
    {
        if (!neighborhoodCompletionStatus.ContainsKey(neighborhoodId))
        {
            neighborhoodCompletionStatus.Add(neighborhoodId, false);
            Debug.Log($"Registered neighborhood ID: {neighborhoodId}");
        }
    }

    // Mark neighborhood as completed
    public void SetNeighborhoodComplete(int neighborhoodId)
    {
        if (neighborhoodCompletionStatus.ContainsKey(neighborhoodId))
        {
            neighborhoodCompletionStatus[neighborhoodId] = true;
            Debug.Log($"Neighborhood ID: {neighborhoodId} marked as completed");
        }
        else
        {
            Debug.LogWarning($"Tried to complete unregistered neighborhood ID: {neighborhoodId}");
        }
    }

    // Check if neighborhood is completed
    public bool IsNeighborhoodComplete(int neighborhoodId)
    {
        if (neighborhoodCompletionStatus.ContainsKey(neighborhoodId))
        {
            return neighborhoodCompletionStatus[neighborhoodId];
        }
        return false;
    }

    // Reset a neighborhood's completion status (for reuse)
    public void ResetNeighborhoodStatus(int neighborhoodId)
    {
        if (neighborhoodCompletionStatus.ContainsKey(neighborhoodId))
        {
            neighborhoodCompletionStatus[neighborhoodId] = false;
            Debug.Log($"Reset neighborhood ID: {neighborhoodId}");
        }
    }
    
    // Debug method to log the completion status of all neighborhoods
    public void LogAllCompletionStatus()
    {
        Debug.Log("=== NEIGHBORHOOD COMPLETION STATUS ===");
        if (neighborhoodCompletionStatus.Count == 0)
        {
            Debug.Log("No neighborhoods registered");
        }
        else
        {
            foreach (var pair in neighborhoodCompletionStatus)
            {
                Debug.Log($"Neighborhood ID {pair.Key}: {(pair.Value ? "COMPLETED" : "incomplete")}");
            }
        }
        Debug.Log("=====================================");
    }
}