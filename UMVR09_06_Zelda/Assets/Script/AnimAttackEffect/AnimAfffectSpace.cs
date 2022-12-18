using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimAfffectSpace : MonoBehaviour
{
    public GameObject FX_Attact0101;
    public GameObject FX_Attact0102;
    public GameObject FX_Attact02;
    public GameObject FX_Attact0301;
    public GameObject FX_Attact0302;
    public GameObject FX_Attact0401;
    public GameObject FX_Attact0402;
    public GameObject FX_AttactSkill01;
    public GameObject FX_AttactSkill0201;
    public GameObject FX_AttactSkill0202;
    public GameObject FX_AttactSkill0301;
    public GameObject FX_AttactSkill0302;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    void Attact0101()
    {
        FX_Attact0101.GetComponent<ParticleSystem>().Play();
    }
    void Attact0102()
    {
        FX_Attact0102.GetComponent<ParticleSystem>().Play();
    }
    void Attact02()
    {
        FX_Attact02.GetComponent<ParticleSystem>().Play();
    }
    void Attact0301()
    {
        FX_Attact0301.GetComponent<ParticleSystem>().Play();
    }
    void Attact0302()
    {
        FX_Attact0302.GetComponent<ParticleSystem>().Play();
    }
    void Attact0402()
    {
        FX_Attact0402.GetComponent<ParticleSystem>().Play();
    }
    void AttactSkill01()
    {
        FX_AttactSkill01.GetComponent<ParticleSystem>().Play();
    }
    void AttactSkill0201()
    {
        FX_AttactSkill0201.GetComponent<ParticleSystem>().Play();
    }
    void AttactSkill0202()
    {
        FX_AttactSkill0202.GetComponent<ParticleSystem>().Play();
    }
    void AttactSkill0301()
    {
        FX_AttactSkill0301.GetComponent<ParticleSystem>().Play();
    }
    void AttactSkill0302()
    {
        FX_AttactSkill0302.GetComponent<ParticleSystem>().Play();
    }
}
