using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonEffect : MonoBehaviour
{
    public GameObject FX_Yell;
    public GameObject FX_PerpareFireBall;
    public GameObject FX_Ascent;
    public GameObject FX_ThrewFire;

    void Yell()
    {
        FX_Yell.GetComponent<ParticleSystem>().Play();
        FX_Yell.GetComponent<AudioSource>().Play();
    }

    void PerpareFireBall()
    {
        FX_PerpareFireBall.GetComponent<ParticleSystem>().Play();
    }

    void AscentEffect()
    {
        FX_Ascent.GetComponent<ParticleSystem>().Play();
    }

    void ThrewFireEffect()
    {
        FX_ThrewFire.GetComponent<ParticleSystem>().Play();
    }

}
