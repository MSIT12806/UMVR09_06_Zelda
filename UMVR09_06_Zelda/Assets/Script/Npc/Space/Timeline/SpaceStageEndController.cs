using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        StartCoroutine(ShowSuccess());
        //camera1.gameObject.SetActive(false);
        //camera2.gameObject.SetActive(false);

        //this.gameObject.SetActive(false);
    }

    IEnumerator ShowSuccess()
    {
        yield return new WaitForSeconds(2);
        UiManager.singleton.ShowSuccess();
        yield return new WaitForSeconds(3);
        BlackScreen.FadeOut(0.02f);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);

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
