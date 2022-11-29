using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordLight_Main : MonoBehaviour
{
    public GameObject AttackEffect01;

    private Animation anim;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.IsName("Attack01") && stateInfo.normalizedTime < 0.5f)
        {
                AttackEffect01.SetActive(true);
        }
        else if(stateInfo.IsName("Attack01") == false)
        {
            AttackEffect01.SetActive(false);
        }
    }
}
