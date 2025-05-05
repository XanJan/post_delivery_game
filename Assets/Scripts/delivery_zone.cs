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
            if(player.TryPeekInteraction(out var res))
            {
                switch(res)
                {
                    case pickup_interactable:
                    if(detectedPackages.Count < maxPackages && player.TryForceInteractEnd(null,out var top))
                    {
                        TryConsumePackage(top.gameObject);
                    }
                    break;
                }
            }
            
        }
    }
    /// <summary>
    /// Helper method, tries to consumes a package. If zone is not full, 
    /// update score and text and place package on zone. If the zone becomes 
    /// full, change color and notify current game event.
    /// </summary>
    /// <param name="go">GameObject to "consume" and Destroy.</param>
    private void TryConsumePackage(GameObject go)
    {
        if(detectedPackages.Count < maxPackages && !detectedPackages.Contains(go))
        {
            float lastYPos = detectedPackages.Count > 0 ? (detectedPackages.Last().transform.localPosition.y + (detectedPackages.Last().TryGetComponent<BoxCollider>(out var collider2) ? collider2.size.y : 1) ) : 0;
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
                go.transform.SetParent(transform);
                go.transform.eulerAngles =new Vector3(0,new int[]{0,90,180,270}[detectedPackages.Count % 4],0);
                go.transform.localPosition = (_dropOffPoints!=null && _dropOffPoints.Count>0) ? 
                    _dropOffPoints[detectedPackages.Count-1 % _dropOffPoints.Count].localPosition : 
                        new Vector3(new float[]{0.1f,0f,-0.1f,0f}[detectedPackages.Count%4], lastYPos ,new float[]{0f,0.1f,0f,-0.1f}[detectedPackages.Count%4]);
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
