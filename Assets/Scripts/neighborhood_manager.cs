using System.Collections.Generic;
using UnityEngine;

public class neighborhood_manager : MonoBehaviour
{
    [SerializeField] private int neighborhoodId; // identifier for this neighborhood
    [SerializeField] private Transform deliveryZonesParent; // Parent transform containing all delivery zones
    
    private int totalZones;
    private int completedZones = 0;
    private bool isCompleted = false;
    private List<delivery_zone> deliveryZones = new List<delivery_zone>();
    private int deliveryZonesCount;
    private int finishedDeliveryZones;
    void Start()
    {
        deliveryZonesCount = this.transform.childCount;
        Debug.Log(deliveryZones);

        game_events.current.onPackageTrigger += CountFinishedDeliveryZones;
        game_events.current.RaiseNeighborhoodGenerated();
    }

    private void CountFinishedDeliveryZones()
    {
        finishedDeliveryZones++;
        if (deliveryZonesCount <= finishedDeliveryZones)
        // Register neighborhood with the tracker
        neighborhood_delivery_tracker.Instance.RegisterNeighborhood(neighborhoodId);
        
        // Find all delivery zones in neighborhood
        if (deliveryZonesParent == null)
        {
            // Try to find the delivery zones parent by name
            Transform[] transforms = GetComponentsInChildren<Transform>();
            foreach (Transform t in transforms)
            {
                if (t.name.Contains("DELIVERY ZONES"))
                {
                    deliveryZonesParent = t;
                    break;
                }
            }
        }

        if (deliveryZonesParent != null)
        {
            // Find and store all delivery zones in this neighborhood
            foreach (Transform child in deliveryZonesParent)
            {
                delivery_zone zone = child.GetComponent<delivery_zone>();
                if (zone != null)
                {
                    totalZones++;
                }
            }
            
            Debug.Log($"Neighborhood {neighborhoodId} has {totalZones} delivery zones");
        }
        else
        {
            Debug.LogWarning($"No delivery zones parent found in neighborhood {neighborhoodId}");
        }
    }
    
    // This method allows delivery zones to register themselves with this manager
    public void RegisterDeliveryZone(delivery_zone zone)
    {
        if (!deliveryZones.Contains(zone))
        {
            deliveryZones.Add(zone);
            
            // Only increment the counter if we didn't find these zones 
            if (deliveryZonesParent == null || !zone.transform.IsChildOf(deliveryZonesParent))
            {
                totalZones++;
                Debug.Log($"Neighborhood {neighborhoodId}: Registered zone {zone.name}, now tracking {totalZones} zones");
            }
        }
    }

    // called when a delivery zone is completed
    public void ZoneCompleted(delivery_zone zone)
    {
        // Verify zone belongs to this neighborhood
        if (!deliveryZones.Contains(zone))
        {
            Debug.LogWarning($"Zone {zone.name} reported completion to neighborhood {neighborhoodId} but isn't part of this neighborhood");
            return;
        }
        
        completedZones++;
        Debug.Log($"Neighborhood {neighborhoodId}: {completedZones}/{totalZones} zones completed");
        
        // Check if all delivery zones in neighborhood are completed
        if (totalZones > 0 && completedZones >= totalZones && !isCompleted)
        {
            isCompleted = true;
            
            // Mark neighborhood as completed in tracker
            neighborhood_delivery_tracker.Instance.SetNeighborhoodComplete(neighborhoodId);
            
            // Notify that neighborhood is finished
            game_events.current.NeighborhoodFinished();
            
            Debug.Log($"Neighborhood {neighborhoodId} is now complete!");
        }
    }
    public int GetNeighborhoodId()
    {
        return neighborhoodId;
    }
}
