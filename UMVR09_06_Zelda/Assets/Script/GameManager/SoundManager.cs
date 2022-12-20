using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager singleton;
    //AudioClip
    [HideInInspector] public AudioClip LicoNormalAttack;
    [HideInInspector] public AudioClip DragonRoar;



    AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        singleton = this;

        //injection AudioClips
        LicoNormalAttack = (AudioClip)Resources.Load("Sounds/metal-clank-5");

        //....
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            PlaySoundTest();
        }
    }

    public void PlaySound(Vector3 soundPosition, float maxDistance)
    {
        var listenerPos = transform.position;
        //測量距離

        //播放...比方說，龍龍袍校
        source.PlayOneShot(DragonRoar);
    }
    public void PlaySoundTest()
    {
        var listenerPos = transform.position;
        //測量距離

        //播放...比方說，龍龍袍校
        source.PlayOneShot(LicoNormalAttack);
    }
}
