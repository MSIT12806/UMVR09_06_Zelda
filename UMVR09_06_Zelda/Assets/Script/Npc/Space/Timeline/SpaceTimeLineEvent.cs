using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceTimeLineEvent : MonoBehaviour
{
    public GameObject spaceCamera1;
    public GameObject spaceCamera2;
    public GameObject spaceCamera3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StopCamera1()
    {
        spaceCamera1.SetActive(false);
    }
    public void StopCamera2()
    {
        spaceCamera2.SetActive(false);

    }
    public void StopCamera3()
    {
        spaceCamera3.SetActive(false);

    }
}
