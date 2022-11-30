using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsaoManager : MonoBehaviour, IHp, NpcHelper
{

    //test
    //public float flyTime = 0;
    //public bool knock = false;
    //public float flyHigh = 0;
    //public float flyDis = 0;

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
        //-------------------
        //if (knock)
        //{
        //    flyTime += Time.deltaTime;
        //    if(flyTime >= 0.3) knock = false;
        //    if (flyTime <= 0.15) flyHigh = flyHigh * 2;
        //    else flyHigh = flyHigh / 2;

        //    transform.Translate(0, flyHigh, -0.2f);
        //}
        //--------------------
    }

    //public void ToKnock(float a)//test
    //{
    //    flyHigh = a;
    //    knock = true;
    //}

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