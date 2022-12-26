using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCloser : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject virCamera;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseCamera()
    {
        virCamera.SetActive(false);
    }
    public void OpenCamera()
    {
        virCamera.SetActive(true);
    }

    public void FinalLight()
    {
       var fx = GameObject.Find("fbxPodium (1)").transform.FindAnyChild<Transform>("FX_Light_Line");
        fx.gameObject.SetActive(true);
    }

    public void NFirstFinish()
    {
        var ps = ObjectManager.MainCharacter.GetComponent<PicoState>();
        ps.LevelOneFinished();
    }
    public void NTwoFinish()
    {
        var ps = ObjectManager.MainCharacter.GetComponent<PicoState>();
        ps.LevelTwoFinished();
    }
    public void NThreeFinish()
    {
        var ps = ObjectManager.MainCharacter.GetComponent<PicoState>();
        ps.LevelThreeFinished();
    }
}
