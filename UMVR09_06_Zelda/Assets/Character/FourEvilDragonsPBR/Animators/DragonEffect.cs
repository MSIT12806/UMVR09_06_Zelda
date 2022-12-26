using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonEffect : MonoBehaviour
{
    public GameObject FX_Yell;
    public GameObject FX_PerpareFireBall;
    public GameObject FX_Ascent;
    public GameObject FX_ThrewFire;
    public GameObject FX_TailAttack;
    public GameObject FX_RunAttack;
    public GameObject FX_StopRun;
    public GameObject FX_Bit;

    public AudioSource SE_FlyWing;
    public AudioSource SE_PrepRun;
    public AudioSource SE_FlyAttack;

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

    void TailAttackEffect()
    {
        FX_TailAttack.GetComponent<ParticleSystem>().Play();
    }

    void RunAttackEffect()
    {
        FX_RunAttack.GetComponent<ParticleSystem>().Play();
    }

    void FX_StopRunkEffect()
    {
        FX_StopRun.GetComponent<ParticleSystem>().Play();
    }

    void FX_BitEffect()
    {
        FX_Bit.GetComponent<ParticleSystem>().Play();
    }


    void FlyWingSound()
    {
        SE_FlyWing.GetComponent<AudioSource>().Play();
    }

    void PrepRunSound()
    {
        SE_PrepRun.GetComponent<AudioSource>().Play();
    }

    void FlyAttackSound()
    {
        SE_FlyAttack.GetComponent<AudioSource>().Play();
    }
}
