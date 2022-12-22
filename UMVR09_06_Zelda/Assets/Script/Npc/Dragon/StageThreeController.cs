using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageThreeController : MonoBehaviour
{

    public GameObject golem1;
    public GameObject golem2;
    GolemManager golem;
    public CinemachineVirtualCamera camera;
    public AudioSource GolemBGM;
    public void StageThreeShowFinished()
    {
        Destroy(golem1);
        golem2.SetActive(true);
        golem = golem2.GetComponent<GolemManager>();

        camera.Priority = 5;
        camera.gameObject.SetActive(false);
        golem.Stand = true;
    }
    public void PlayGolemBGM()
    {
        GolemBGM.Play();
    }

    public void OpenUi()
    {
        UiManager.singleton.gameObject.SetActive(true);
    }
    public void CloseUi()
    {
        UiManager.singleton.gameObject.SetActive(false);
    }
}
