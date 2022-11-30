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

    #region all aistate
    public UsaoIdleState usaoIdleState;
    public UsaoFightState usaoFightState;
    public UsaoChaseState usaoChaseState;
    public UsaoAttackState usaoAttackState;
    public UsaoHurtState usaoHurtState;
    public UsaoDeathState usaoDeathState;
    #endregion
    public float Hp { get => npc.Hp; set => npc.Hp = value; }
    void Awake()
    {
        ObjectManager.StateManagers.Add(this.gameObject.GetInstanceID(), this);
        animator = transform.GetComponent<Animator>();
    }
    void Start()
    {
        var picoState = ObjectManager.MainCharacter.GetComponent<PicoState>();
        usaoIdleState = new UsaoIdleState(ObjectManager.MainCharacter, picoState, animator, transform, this);
        usaoFightState = new UsaoFightState(ObjectManager.MainCharacter, animator, transform, this);
        //usaoChaseState = 
        //    usaoAttackState
        //    usaoHurtState
        //    usaoDeathState

        aiState = usaoIdleState;
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
        aiState = new UsaoHurtState(animator, transform, damageData, usaoFightState, this);
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