using Ron;
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


    public Vector3 OriginPosition;
    Transform head;
    //---Ai---//
    AiState aiState;
    Npc npc;
    Animator animator;
    public GameState stage { get => npc.gameState; }
    #region all aistate
    public UsaoIdleState usaoIdleState;
    public UsaoFightState usaoFightState;
    //public UsaoChaseState usaoChaseState;
    //public UsaoAttackState usaoAttackState;
    //public UsaoHurtState usaoHurtState;
    //public UsaoDeathState usaoDeathState;
    #endregion
    public float Hp { get => npc.Hp; set => npc.Hp = value; }

    public bool CanBeKockedOut => throw new NotImplementedException();

    public bool Dizzy => throw new NotImplementedException();

    public float MaxHp => throw new NotImplementedException();

    public float WeakPoint => throw new NotImplementedException();

    public float MaxWeakPoint => throw new NotImplementedException();

    public float Radius => 0.4f;

    public float CollisionDisplacement => 0.06f;

    void Awake()
    {
        ObjectManager.StateManagers.Add(this.gameObject.GetInstanceID(), this);
        animator = transform.GetComponent<Animator>();
        OriginPosition = transform.position;
        head = transform.FindAnyChild<Transform>("Character1_Head");
    }
    void Start()
    {
        npc = transform.GetComponent<Npc>();
        var picoState = ObjectManager.MainCharacter.GetComponent<PicoState>();
        usaoIdleState = new UsaoIdleState(ObjectManager.MainCharacter, picoState, animator, transform, this);
        usaoFightState = new UsaoFightState(ObjectManager.MainCharacter, animator, transform, this);

        aiState = usaoIdleState;
    }

    // Update is called once per frame
    void Update()
    {
        if (aiState == null) print("aiState is null");

        aiState = aiState.SwitchState();
        Move();
        Jump();
    }
    private void LateUpdate()
    {
        aiState.SetAnimation();
    }
    public void GetHurt(DamageData damageData)
    {
        aiState = new UsaoHurtState(animator, transform, damageData, usaoFightState, this);
    }
    public float forward;
    public Vector3 trunDirection;
    public void Move()
    {
        //animator.SetFloat("forward", forward);
    }

    public void Attack()
    {
        NpcCommon.AttackDetection("", transform.position, transform.forward, 15f, 1f, false, new DamageData(5, Vector3.zero, HitType.light, DamageStateInfo.NormalAttack), "Player");
    }
    public void Turn(Vector3 direction)
    {
        if (npc.collide) return;
        var degree = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        if (degree < 0)
        {
            transform.Rotate(Vector3.up, -2);
        }
        else if (degree > 0)
        {
            transform.Rotate(Vector3.up, 2);
        }
    }

    public void Look(Transform targetHead)
    {
        if (UnityEngine.Random.value < 0.99) return;
        var degreeY = Vector3.Angle(transform.forward.WithY(), (targetHead.position - head.position).WithY());
        degreeY = degreeY * Mathf.Sign(Vector3.SignedAngle(transform.forward, targetHead.position - head.position, Vector3.up));
        if (degreeY > 80)
        {
            degreeY = 80;
        }
        else if (degreeY < -80)
        {
            degreeY = -80;
        }
        var degreeX = Vector3.Angle((targetHead.position - head.position).WithY(), (targetHead.position - head.position));
        degreeX = degreeX * Mathf.Sign(Vector3.SignedAngle((targetHead.position - head.position).WithY(), targetHead.position - head.position, -transform.right));
        if (degreeX > 40)
        {
            degreeX = 40;
        }
        else if (degreeX < -40)
        {
            degreeX = -40;
        }
        var d = head.forward.WithY();
        d = Quaternion.AngleAxis(degreeY, Vector3.up) * d;
        d = Quaternion.AngleAxis(degreeX, d.GetLocalRight()) * d;

        //var degree = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        //if (degree < 0)
        //{
        //    transform.Rotate(Vector3.up, -1);
        //}
        //else if (degree > 0)
        //{
        //    transform.Rotate(Vector3.up, 1);
        //}
        head.forward = d.normalized;
    }
    bool jump;
    Vector3 jumpRandomDirection;
    public void Jump()
    {
        if (jump)
        {
            
            transform.position += jumpRandomDirection * 0.05f;
        }
    }
    public void JumpStart()
    {
        jumpRandomDirection = (new Vector3(UnityEngine.Random.value - 0.5f, 0, UnityEngine.Random.value - 0.5f)).normalized;
        jump = true;
    }
    public void JumpEnd()
    {
        jump = false;
    }
}