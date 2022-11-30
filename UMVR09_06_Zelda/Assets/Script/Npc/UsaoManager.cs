using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsaoManager : MonoBehaviour, IHp, NpcHelper
{
    //---Ai---//
    AiState aiState;
    Npc npc;
    Animator animator;
    public float Hp { get => npc.Hp; set => npc.Hp = value; }
    void Awake()
    {
        print(GetInstanceID());
        ObjectManager.StateManagers.Add(this.gameObject.GetInstanceID(), this);
        animator = transform.GetComponent<Animator>();
    }
    void Start()
    {
        aiState = new UsaoIdleState(ObjectManager.MainCharacter, ObjectManager.MainCharacter.GetComponent<PicoState>(), animator, transform);
        npc = transform.GetComponent<Npc>();
    }

    // Update is called once per frame
    void Update()
    {
        aiState.SetAnimation();
        aiState = aiState.SwitchState();
        Move();
    }
    public void GetHurt(DamageData damageData)
    {
        var f = new UsaoFightState(ObjectManager.MainCharacter, animator, transform);
        aiState = new UsaoHurtState(animator, transform, damageData, f);
    }
    public float forward;
    public Vector3 trunDirection;
    public void Move()
    {
        animator.SetFloat("forward", forward);
    }

    public void Turn()
    {
        //轉向  速度？

    }

    public void Turn(Vector3 direction)
    {
        throw new NotImplementedException();
    }
}