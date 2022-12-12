using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordLight_Usao : MonoBehaviour
{
    public GameObject AttackEffect0101;
    public GameObject AttackEffect0102;
    public GameObject AttackEffect0103;
    public GameObject AttackEffect02;

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
}
