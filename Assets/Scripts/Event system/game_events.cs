using UnityEngine;
using System;
using UnityEngine.Rendering;

public class game_events : MonoBehaviour
{
    public static game_events current;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        current = this;
    }

    public event Action onEndLevelEnter;
    public void EndLevelEnter()
    {
        if(onEndLevelEnter != null)
        {
            onEndLevelEnter();
        }
    }

    public event Action onPackageTrigger;
    public void PackageComplete()
    {
        if(onPackageTrigger != null)
        {
            onPackageTrigger();
        }
    }

    public event Action onNeighborhoodFinished;
    public void NeighborhoodFinished()
    {
        if (onNeighborhoodFinished != null)
        {
            onNeighborhoodFinished();
        }
    }
}
