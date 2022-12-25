using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceStageEndController : MonoBehaviour
{
    public GameObject space1;
    public GameObject space2;
    SpaceManager Space;
    public CinemachineVirtualCamera camera1;
    public CinemachineVirtualCamera camera2;
    public AudioSource SpaceBGM;

    BlackFade1 BlackScreen;

    private void Awake()
    {
        BlackScreen = GameObject.Find("BlackScreen").GetComponent<BlackFade1>();
    }
    public void StageTwoShowFinished()
    {
        Destroy(space1);
        Destroy(space2);
        camera1.Priority = 5;
        camera2.Priority = 5;
        Space = space2.GetComponent<SpaceManager>();
        Space.Show = true;

        camera1.gameObject.SetActive(false);
        camera2.gameObject.SetActive(false);

        this.gameObject.SetActive(false);
    }

    public void PlaySpaceBGM()
    {
        //SpaceBGM.Play();
    }

    public void OpenUi()
    {
        BlackScreen.FadeIn(0.05f);
        Debug.Log(111222);
        UiManager.singleton.gameObject.SetActive(true);
    }
    public void CloseUi()
    {
        UiManager.singleton.gameObject.SetActive(false);
    }

}
