using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimAffectUsao : MonoBehaviour
{
    public GameObject YellEffect;
    public GameObject AttackEffect0101;
    public GameObject AttackEffect0102;
    public GameObject AttackEffect0103;

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
}
