using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.Mathematics;
using System.Linq;

public class delivery_zone : MonoBehaviour
{
    private HashSet<GameObject> detectedPackages = new HashSet<GameObject>();
    private Renderer areaRenderer;
    public Color red = Color.red;
    public Color green = Color.green;
    public int maxPackages = 5;
    public TextMeshPro textDisplay;
    /// <summary>
    /// The points where the dropped off packages should appear. 
    /// </summary>
    [SerializeField] private List<Transform> _dropOffPoints;
    void Start(){
        areaRenderer = GetComponent<Renderer>();
        areaRenderer.material.color = red;   
        UpdateText();
    }

    //when a package enters, count it and remove it from the word, turn green and stop when "full"
    void OnTriggerEnter(Collider other){
        if(other.TryGetComponent<pickup_interactable>(out var res))
        {
            if(res.GetInteractors().Length==0) // Noone is interacting with the package
            {
                TryConsumePackage(other.gameObject);
            }
        } 
    }
    /// <summary>
    /// If a player stands on the zone and the zone is not full, try to take a package 
    /// from its active interactions.
    /// </summary>
    void OnTriggerStay(Collider other)
    {
        if(other.TryGetComponent<player_interactor>(out var player))
        {
            if(detectedPackages.Count < maxPackages && player.TryForceInteractEnd(null,out var top))
            {
                TryConsumePackage(top.gameObject);
            }
        }
    }
    /// <summary>
    /// Helper method, tries to consumes a package. If zone is not full, 
    /// update score and text and Destroy package. If the zone becomes 
    /// full, change color and notify current game event.
    /// </summary>
    /// <param name="go">GameObject to "consume" and Destroy.</param>
    private void TryConsumePackage(GameObject go)
    {
        if(detectedPackages.Count < maxPackages && !detectedPackages.Contains(go))
        {
            Vector3 lastpos = detectedPackages.Count > 0 ? detectedPackages.Last().transform.position : transform.position;
            detectedPackages.Add(go);
            score_manager.Instance.AddScore();
            UpdateText();
            if(detectedPackages.Count >= maxPackages){
                areaRenderer.material.color = green;
                try{game_events.current.PackageComplete();} 
                catch(NullReferenceException){Debug.Log("Warning, game_event instance is null. Game events may not register properly.");}
            }
            if (go != null) 
            {
                go.transform.rotation = transform.rotation;
                go.transform.position = (_dropOffPoints!=null && _dropOffPoints.Count>0) ? 
                    _dropOffPoints[detectedPackages.Count-1 % _dropOffPoints.Count].position : 
                        lastpos + (detectedPackages.Count == 1 ? Vector3.zero : (new Vector3(0.1f, go.TryGetComponent<BoxCollider>(out var collider) ? collider.size.y : 1 ,0.1f)));
                if(go.TryGetComponent<interactable_object>(out var interactable))
                {
                    interactable.AllowInteractions = false;
                }
                if(go.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.isKinematic = true;
                }
            }
        }   
    }
    /// <summary>
    /// Helper method, updates text to match detected package count.
    /// </summary>
    private void UpdateText(){
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
