using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordLight_Main : MonoBehaviour
{
    //light
    public GameObject AttackEffect01;
    public GameObject AttackEffect02;
    public GameObject AttackEffect03;
    public GameObject AttackEffect04;

    //heavy
    public GameObject AttackEffectHeavy01;
    public GameObject AttackEffectHeavy02;
    public GameObject AttackEffectHeavy03;
    public GameObject AttackEffectHeavy04;

    private Animator animator;
    private AnimatorStateInfo stateInfo;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        LightAttack01();
        LightAttack02();
        LightAttack03();
        LightAttack04();

        HeavyAttack01();
        HeavyAttack02();
        HeavyAttack03();
        HeavyAttack04();

    }

    void LightAttack01()
    {
        if (stateInfo.IsName("Attack01") && stateInfo.normalizedTime < 0.5f)
        {
            AttackEffect01.SetActive(true);
        }
        else if (stateInfo.IsName("Attack01") == false)
        {
            AttackEffect01.SetActive(false);
        }
    }

    void LightAttack02()
    {
        if (stateInfo.IsName("Attack01 0") && stateInfo.normalizedTime < 0.5f)
        {
            AttackEffect02.SetActive(true);
        }
        else if (stateInfo.IsName("Attack01 0") == false)
        {
            AttackEffect02.SetActive(false);
        }
    }

    void LightAttack03()
    {
        if (stateInfo.IsName("Attack01 1") && stateInfo.normalizedTime > 0.3f)
        {
            AttackEffect03.SetActive(true);
        }
        else if (stateInfo.IsName("Attack01 1") == false)
        {
            AttackEffect03.SetActive(false);
        }
    }

    void LightAttack04()
    {
        if (stateInfo.IsName("Attack01 2") && stateInfo.normalizedTime > 0.3f)
        {
            AttackEffect04.SetActive(true);
        }
        else if (stateInfo.IsName("Attack01 2") == false)
        {
            AttackEffect04.SetActive(false);
        }
    }

    void HeavyAttack01()
    {
        if (stateInfo.IsName("Attack02") && stateInfo.normalizedTime > 0.5f)
        {
            AttackEffectHeavy01.SetActive(true);
        }
        else if (stateInfo.IsName("Attack02") == false)
        {
            AttackEffectHeavy01.SetActive(false);
        }
    }

    void HeavyAttack02()
    {
        if (stateInfo.IsName("Attack02 0") && stateInfo.normalizedTime > 0.5f)
        {
            AttackEffectHeavy02.SetActive(true);
        }
        else if (stateInfo.IsName("Attack02 0") == false)
        {
            AttackEffectHeavy02.SetActive(false);
        }
    }

    void HeavyAttack03()
    {
        if (stateInfo.IsName("Attack02 1") && stateInfo.normalizedTime > 0.4f)
        {
            AttackEffectHeavy03.SetActive(true);
        }
        else if (stateInfo.IsName("Attack02 1") == false)
        {
            AttackEffectHeavy03.SetActive(false);
        }
    }

    void HeavyAttack04()
    {
        if (stateInfo.IsName("Attack02 2") && stateInfo.normalizedTime > 0.4f)
        {
            Debug.Log(stateInfo.normalizedTime);
            AttackEffectHeavy04.SetActive(true);
        }
        else if (stateInfo.IsName("Attack02 2") == false)
        {
            AttackEffectHeavy04.SetActive(false);
        }
    }
}