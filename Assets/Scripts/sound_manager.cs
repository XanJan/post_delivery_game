using UnityEngine;
using UnityEngine.Audio;

public class sound_manager : MonoBehaviour
{
    [Header("___Audio Sources___")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("___Audio Clips___")]
    public AudioClip dropBoxSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        game_events.current.onPacketFall += PlayDropBoxSound;
    }

    private void PlayDropBoxSound()
    {
        Debug.Log("PlayDropBoxSound");
        sfxSource.clip = dropBoxSound;
        sfxSource.Play();
    }

}
