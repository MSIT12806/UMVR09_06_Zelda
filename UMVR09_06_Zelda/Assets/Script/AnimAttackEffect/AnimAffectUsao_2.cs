using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimAffectUsao_2 : MonoBehaviour
{
    public GameObject YellEffect;
    public GameObject AttackEffect0101;
    public GameObject AttackEffect0102;
    public GameObject AttackEffect0201;
    public GameObject AttackEffect0301;

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
        YellEffect.GetComponent<ParticleSystem>().Play();
    }
    void Effect0101()
    {
        AttackEffect0101.GetComponent<ParticleSystem>().Play();
    }
    void Effect0102()
    {
        AttackEffect0102.GetComponent<ParticleSystem>().Play();
    }
    void Effect0201()
    {
        AttackEffect0201.GetComponent<ParticleSystem>().Play();
    }
    void Effect0301()
    {
        AttackEffect0301.GetComponent<ParticleSystem>().Play();
    }
}
