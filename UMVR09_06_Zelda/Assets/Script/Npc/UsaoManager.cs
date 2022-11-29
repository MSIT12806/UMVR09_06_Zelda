using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsaoManager : MonoBehaviour, IHp
{
    //---Ai---//
    AiState aiState;
    Npc npc;
    public float Hp { get => npc.Hp; set => npc.Hp = value; }
    void Awake()
    {
        print(GetInstanceID());
        ObjectManager.StateManagers.Add(this.gameObject.GetInstanceID(), this);
    }
    void Start()
    {
        aiState = new UsaoIdleState(ObjectManager.MainCharacter, ObjectManager.MainCharacter.GetComponent<PicoState>(), transform.GetComponent<Animator>(), transform);
        npc = transform.GetComponent<Npc>();
    }

    // Update is called once per frame
    void Update()
    {
        aiState.SetAnimation();
        aiState = aiState.SwitchState();
    }
    public void GetHurt(DamageData damageData)
    {
        var f = new UsaoFightState(ObjectManager.MainCharacter, transform.GetComponent<Animator>(), transform);
        aiState = new UsaoHurtState(transform.GetComponent<Animator>(), transform, damageData, f);
    }
}