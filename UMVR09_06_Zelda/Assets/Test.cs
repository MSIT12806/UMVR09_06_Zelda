using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    ParticleSystem[] a;
    // Start is called before the first frame update
    void Start()
    {
        a = this.transform.GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("A");

                a[0].Play();
            
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("S");

                a[0].Stop();
        }
    }
}
