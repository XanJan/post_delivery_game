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
    [SerializeField] private List<AudioClip> soundClips;
    private static SoundManager instance;
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1.0f)
    {
        instance.audioSource.PlayOneShot(instance.soundClips[(int)sound], volume);
    }


}