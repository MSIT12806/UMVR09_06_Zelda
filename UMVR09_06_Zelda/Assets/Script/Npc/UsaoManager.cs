using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsaoManager : MonoBehaviour
{
    //---Ai---//
    AiState aiState;
    void Start()
    {
        aiState = new UsaoIdleState(ObjectManager.MainCharacter, ObjectManager.MainCharacter.GetComponent<PicoState>(), transform.GetComponent<Animator>(), transform);
    }

    // Update is called once per frame
    void Update()
    {
        aiState.SetAnimation();
        aiState = aiState.SwitchState();
    }
    public void GetHurt(DamageData damageData)
    {
        aiState = new UsaoHurtState(transform.GetComponent<Animator>(), transform, damageData);
    }
}