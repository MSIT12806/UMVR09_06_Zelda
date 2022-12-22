using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonSoundManager : MonoBehaviour
{
    AudioSource ads;

    int state = 1;//1=> keep. 0=> close. 2=> open
    // Start is called before the first frame update
    void Start()
    {
        ads = GetComponent<AudioSource>();
    }
    float duration = 2.5f;
    float currentTime = 0;
    // Update is called once per frame
    void Update()
    {
        if (currentTime > duration) return;
        currentTime += Time.deltaTime;
        Debug.Log("volume = "+ ads.volume);
        switch (state)
        {
            case 1:
            default:
                return;
            case 0:
                ads.volume = Mathf.Lerp(ads.volume, 0, currentTime / duration);
                return;
            case 2:
                ads.volume = Mathf.Lerp(ads.volume, 0.2f, currentTime / duration);
                return;
        }
    }


    IEnumerator PlayMusic()
    {
        yield return new WaitForSeconds(1.5f);
        ads.Play();
        yield return 0;
    }

    public void StopSceneMusic()
    {
        currentTime = 0;
        state = 0;
    }
    private void OpenSceneMusic()
    {
        currentTime = 0;
        state = 2;
    }
}
