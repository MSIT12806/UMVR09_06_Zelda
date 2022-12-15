using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonEffect : MonoBehaviour
{
    public GameObject FX_Yell;
    public GameObject FX_PerpareFireBall;
    public GameObject FX_Ascent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Yell()
    {
        FX_Yell.GetComponent<ParticleSystem>().Play();
    }

    void PerpareFireBall()
    {
        FX_PerpareFireBall.GetComponent<ParticleSystem>().Play();
    }

    void AscentEffect()
    {
        FX_Ascent.GetComponent<ParticleSystem>().Play();
    }

}
