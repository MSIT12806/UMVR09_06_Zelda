using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSoundManager : MonoBehaviour
{
    AudioSource ads;
    // Start is called before the first frame update
    void Start()
    {
        ads = GetComponent<AudioSource>();
        StartCoroutine(PlayMusic());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PlayMusic()
    {
        yield return new WaitForSeconds(1.5f);
        ads.Play();
        yield return 0;
    }
}
