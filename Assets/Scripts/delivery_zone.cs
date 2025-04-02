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
            if (detectedPackages.Count >= maxPackages){
                return;
            }

            detectedPackages.Add(other.gameObject);
            score_manager.Instance.AddScore();
            UpdateText();

            if(detectedPackages.Count >= maxPackages){
                areaRenderer.material.color = green;
            }

            if (other.gameObject != null) Destroy(other.gameObject); 
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
