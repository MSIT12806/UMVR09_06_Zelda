using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordLight_Usao : MonoBehaviour
{
    public GameObject AttackEffect0101;
    public GameObject AttackEffect0102;
    public GameObject AttackEffect0103;
    public GameObject AttackEffect0201;
    public GameObject AttackEffect0202;
    public GameObject AttackEffect0301;
    public GameObject AttackEffect0302;
    public GameObject AttackEffect0401;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Effect0101() 
    {
        AttackEffect0101.GetComponent<ParticleSystem>().Play();
    }
    void Effect0102()
    {
        AttackEffect0102.GetComponent<ParticleSystem>().Play();
    }
    void Effect0103()
    {
        AttackEffect0103.GetComponent<ParticleSystem>().Play();
    }
    void Effect0201()
    {
        AttackEffect0201.GetComponent<ParticleSystem>().Play();
    }
    void Effect0202()
    {
        AttackEffect0202.GetComponent<ParticleSystem>().Play();
    }
    void Effect0301()
    {
        AttackEffect0301.GetComponent<ParticleSystem>().Play();
    }
    void Effect0302()
    {
        AttackEffect0302.GetComponent<ParticleSystem>().Play();
    }
    void Effect0401()
    {
        AttackEffect0401.GetComponent<ParticleSystem>().Play();
    }
}
