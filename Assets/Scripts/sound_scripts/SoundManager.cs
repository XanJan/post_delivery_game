using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManager>();
                if (_instance == null)
                {
                    GameObject soundManagerObject = new GameObject("SoundManager");
                    _instance = soundManagerObject.AddComponent<SoundManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }
            return _instance;
        }
    }
    public class SoundEffect
    {
        public string name;
        public AudioClip clip;
        public float volume = 1.0f;
        public float pitch = 1.0f;
        public bool loop = false;
        [HideInInspector] public AudioSource source;
    }

    public SoundEffect[] soundEffects;
    
}
