using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnEffect : MonoBehaviour
{
    public float DelayTime = 0f;
    // Start is called before the first frame update
    private AudioSource audioSource;
    private ParticleSystem effect;
    bool CanPlaySound;
    bool Initialized;
    void Start()
    {
        if (!Initialized)
        {
            Initialize();
        }
    }
    void Initialize()
    {
        audioSource = this.GetComponent<AudioSource>();
        effect = this.GetComponent<ParticleSystem>();
        Initialized = true;
    }
    void OnEnable()
    {
        if (!Initialized)
        {
            Initialize();
        }
        if (audioSource != null)
        {
            CanPlaySound = true;
        }
    }
    // Update is called once per framebool CanPlaySound;
    void Update()
    {
        if (effect.isPlaying && CanPlaySound && audioSource != null)
        {
            audioSource.PlayDelayed(DelayTime);
            CanPlaySound = false;
        }
        else if (effect.isStopped && audioSource != null)
        {
            CanPlaySound = true;
        }
    }
}
