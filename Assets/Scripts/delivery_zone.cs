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
    int yOffset = 0;
    float lastYPos;
    public Color red = Color.red;
    public Color green = Color.green;
    public int maxPackages = 5;
    public TextMeshPro textDisplay;
    private enum packageType
    {
        small,
        big
    }

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
        if (other.TryGetComponent<pickup_interactable>(out var res))
        {
            if (res.GetInteractors().Length == 0) // Noone is interacting with the package
            {
                packageType package = packageType.small;
                TryConsumePackage(other.gameObject, package);
            }
        }

        else if (other.TryGetComponent<big_package_interactable>(out var beegRes))
        {
            if (beegRes.GetInteractors().Length == 0)
            {
                packageType package = packageType.big;
                TryConsumePackage(other.gameObject, package);
            }
        }
    }
    /// <summary>
    /// If a player stands on the zone and the zone is not full, try to take a package 
    /// from its active interactions.
    /// </summary>
    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<player_interactor>(out var player))
        {
            if (player.TryPeekInteraction(out var res))
            {
                switch (res)
                {
                    case pickup_interactable:
                    if (detectedPackages.Count < maxPackages && player.TryForceInteractEnd(null, out var top))
                    {
                        packageType package = packageType.small;
                        TryConsumePackage (top.gameObject, package);
                    }
                    break;
                }
            }  
        }
        
        else if (other.TryGetComponent<player_interactor>(out var beegPlayer))
        {
            if (player.TryPeekInteraction(out var beegRes))
            {
                switch (beegRes)
                {
                    case pickup_interactable:
                        if (detectedPackages.Count < maxPackages && beegPlayer.TryForceInteractEnd(null, out var top))
                        {
                            packageType package = packageType.big;
                            TryConsumePackage(top.gameObject, package);
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
    private void TryConsumePackage(GameObject go, packageType pType)
    {
        if (detectedPackages.Count < maxPackages && !detectedPackages.Contains(go))
        {
            lastYPos = (float)(0.5 * (detectedPackages.Count + yOffset));
            detectedPackages.Add(go);
            switch (pType)
            {
                case packageType.small:
                    score_manager.Instance.AddScore(1);
                    break;
                case packageType.big:
                    score_manager.Instance.AddScore(5);
                    break;

            }
            
            UpdateText();
            if(detectedPackages.Count >= maxPackages){
                areaRenderer.material.color = green;
                try{game_events.current.PackageComplete();} 
                catch(NullReferenceException){Debug.Log("Warning, game_event instance is null. Game events may not register properly.");}
            }
            
            if (go != null) 
            {
                go.transform.SetParent(transform);
                go.transform.eulerAngles = new Vector3(0,new int[]{0, 90, 180, 270}[detectedPackages.Count % 4], 0);
                go.transform.localScale = new Vector3(4, 1, 4);
                switch (pType)
                {
                    case packageType.small:
                        go.transform.localPosition = (_dropOffPoints != null && _dropOffPoints.Count > 0) ?
                        _dropOffPoints[detectedPackages.Count - 1 % _dropOffPoints.Count].localPosition :
                        new Vector3(new float[] { 0.1f, 0f, -0.1f, 0f }[detectedPackages.Count % 4], lastYPos, new float[] { 0f, 0.1f, 0f, -0.1f }[detectedPackages.Count % 4]);
                        break;
                    case packageType.big:
                        UpdateYPos();
                        go.transform.localPosition = new Vector3(+0, +lastYPos, +0);
                        yOffset += 1;
                        break;
                }
                
                
                    
                if (go.TryGetComponent<interactable_object>(out var interactable))
                {
                    interactable.AllowInteractions = false;
                }
                
                if (go.TryGetComponent<Rigidbody>(out var rb))
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

    private void UpdateYPos()
    {
        yOffset += 0;
        lastYPos = (float)(0.5 * (detectedPackages.Count + yOffset));
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
