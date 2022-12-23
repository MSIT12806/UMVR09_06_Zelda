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
}
