using UnityEngine;
using System.Collections.Generic;

public enum SoundType
{
    ButtonClick,
    GateOpenSuccess,
    GateOpenFail,
    PackageDelivered,
    DeliveryZoneOneCompleted,
    DeliveryZoneTwoCompleted,
    DeliveryZoneThreeCompleted,
    DeliveryZoneFourCompleted,
    NeighborhoodCompleted,
    PackageThrown,
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundClips;
    private static SoundManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject); 
        
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1.0f)
    {
        if (instance == null)
        {
            Debug.LogError("SoundManager instance is null!");
            return;
        }
        
        if (instance.soundClips.Length <= (int)sound)
        {
            Debug.LogError($"Sound clip for {sound} is not assigned in the inspector!");
            return;
        }
        
        instance.audioSource.PlayOneShot(instance.soundClips[(int)sound], volume);
    }

    public static void PlayZoneCompletionSound(int zoneNumber)
    {
        if (zoneNumber < 1 || zoneNumber > 4)
        {
            Debug.LogError($"Invalid zone number: {zoneNumber}. Should be 1-4.");
            return;
        }
        
        SoundType sound = SoundType.DeliveryZoneOneCompleted + (zoneNumber - 1);
        PlaySound(sound);
    }
}