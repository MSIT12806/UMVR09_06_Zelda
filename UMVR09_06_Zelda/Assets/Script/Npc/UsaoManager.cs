using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsaoManager : MonoBehaviour
{
    // Start is called before the first frame update
    bool idleSwitch;// trigger IdelSwitch
    bool attack;// trigger attack
    bool findTarget;
    bool notReach;
    int attackWay;
    Animator animator;
    //---Ai---//
    AiState aiState;
    void Start()
    {
        animator = GetComponent<Animator>();
        aiState = new IdleState(ObjectManager.MainCharacter, ObjectManager.MainCharacter.GetComponent<PicoState>(), transform.GetComponent<Animator>(), transform);
    }

    // Update is called once per frame
    void Update()
    {
        aiState.SetAnimation();
        aiState = aiState.SwitchState();
    }
    public void GetHurt(DamageData damageData)
    {
        aiState.getHit = damageData;
    }
}