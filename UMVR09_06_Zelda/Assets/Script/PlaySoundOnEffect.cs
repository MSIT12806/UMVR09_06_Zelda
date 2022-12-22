using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnEffect : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource audioSource;
    private ParticleSystem effect;
    bool CanPlaySound;
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        effect = this.GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {

        if (audioSource == null)
        {
        }
    }
    // Update is called once per framebool CanPlaySound;
    void Update()
    {
        if(name == "FX_Yell")
        {
            Debug.Log("Yell"+effect.isPlaying+":" +audioSource + ":"+ CanPlaySound);
        }
        if (effect.isPlaying && CanPlaySound && audioSource != null)
        {
            audioSource.Play();
            //audioSource.PlayDelayed(UnityEngine.Random.Range(0, 0.04f));
            CanPlaySound = false;
        }
        else if (effect.isStopped && audioSource != null)
        {
            CanPlaySound = true;
        }
    }
}
