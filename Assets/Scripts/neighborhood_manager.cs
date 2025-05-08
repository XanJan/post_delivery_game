using UnityEngine;

public class neighborhood_manager : MonoBehaviour
{
    private int deliveryZones;
    private int finishedDeliveryZones = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deliveryZones = this.transform.childCount;
        Debug.Log(deliveryZones);

        game_events.current.onPackageTrigger += CountFinishedDeliveryZones;
        
        game_events.current.RaiseNeighborhoodGenerated();
    }

    private void CountFinishedDeliveryZones()
    {
        finishedDeliveryZones++;
        if (deliveryZones <= finishedDeliveryZones)
        {
            game_events.current.NeighborhoodFinished();
        }
    }

}
