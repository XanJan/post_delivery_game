using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class delivery_zone : MonoBehaviour
{
    private HashSet<GameObject> detectedPackages = new HashSet<GameObject>();
    private Renderer areaRenderer;
    public Color red = Color.red;
    public Color green = Color.green;
    public int maxPackages = 5;
    public TextMeshPro textDisplay;
    void Start(){
        areaRenderer = GetComponent<Renderer>();
        areaRenderer.material.color = red;   
        UpdateText();
    }

    //when a package enters, count it and remove it from the word, turn green and stop when "full"
    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Package") && !detectedPackages.Contains(other.gameObject)){
            if(other.TryGetComponent<pickup_interactable>(out var res))
            {
                if(res.ActiveInteractorsCount() == 0)
                {
                    if (detectedPackages.Count >= maxPackages){
                        return;
                    }

                    detectedPackages.Add(other.gameObject);
                    score_manager.Instance.AddScore();
                    stats_manager.Instance.IncIntStat("Total Score",1);
                    UpdateText();
                    

                    if(detectedPackages.Count >= maxPackages){
                        areaRenderer.material.color = green;
                        game_events.current.PackageComplete();
                    }
                    
                    if (other.gameObject != null) Destroy(other.gameObject); 
                }
            }
            
        }
        if(other.CompareTag("Player") && detectedPackages.Count < maxPackages)
        {
            if(other.TryGetComponent<player_interactor>(out var res))
            {
                res.TryEndInteraction();
            }
        }
    }

    void UpdateText(){
        if(textDisplay != null){
            textDisplay.text = $"{detectedPackages.Count}/{maxPackages}";
        }
    }

    //way to sum total score in a gamehandler (add way of counting scores for different types of packages)
    public int FinalZoneScore(){
        if(detectedPackages.Count >= maxPackages){
            return detectedPackages.Count;
        }
        else{ 
            return 0;
        }
    }
}
