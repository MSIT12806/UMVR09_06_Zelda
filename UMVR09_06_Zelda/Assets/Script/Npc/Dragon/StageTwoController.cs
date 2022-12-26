using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTwoController : MonoBehaviour
{

    public GameObject dragon1;
    public GameObject dragon2;
    DragonManager Dragon;
    public CinemachineVirtualCamera camera;
    public AudioSource DragonBGM;
    public void StageTwoShowFinished()
    {
        Destroy(dragon1);
        dragon2.SetActive(true);
        camera.Priority = 5;
        Dragon = dragon2.GetComponent<DragonManager>();
        Dragon.Show = true;

        camera.gameObject.SetActive(false);
    }

    public void PlayDragonBGM() 
    {
        DragonBGM.Play();
    }

    public void OpenUi()
    {
        UiManager.singleton.gameObject.SetActive(true);
    }
    public void CloseUi()
    {
        StartSoundManager.singleton.StopSceneMusic();
        UiManager.singleton.gameObject.SetActive(false);
    }
}
