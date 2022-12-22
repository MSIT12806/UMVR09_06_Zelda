using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceStageController : MonoBehaviour
{
    public GameObject space1;
    public GameObject space2;
    SpaceManager Space;
    public CinemachineVirtualCamera camera1;
    public CinemachineVirtualCamera camera2;
    public CinemachineVirtualCamera camera3;
    public AudioSource SpaceBGM;

    public Transform PicoTpPlace;
    public GameObject Pico;
    public void StageTwoShowFinished()
    {
        Destroy(space1);
        space2.SetActive(true);
        camera1.Priority = 5;
        camera2.Priority = 5;
        camera3.Priority = 5;
        Space = space2.GetComponent<SpaceManager>();
        Space.Show = true;

        camera1.gameObject.SetActive(false);
        camera2.gameObject.SetActive(false);
        camera3.gameObject.SetActive(false);

        this.gameObject.SetActive(false);
    }

    public void PlaySpaceBGM()
    {
        //SpaceBGM.Play();
    }

    public void OpenUi()
    {
        UiManager.singleton.gameObject.SetActive(true);
    }
    public void CloseUi()
    {
        //Pico.transform.Translate(0, 0, 5);
        //Pico.transform.position = PicoTpPlace.position;
        UiManager.singleton.gameObject.SetActive(false);
    }

}
