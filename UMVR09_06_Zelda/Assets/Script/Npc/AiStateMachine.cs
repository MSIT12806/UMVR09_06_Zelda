using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BasicAi
{
    //Ai 的基本構成
    // 1. 感官
    //碰撞體
    //視線
    //領地

    // 2. 表達
    //轉向速度極限
    //前進速度極限(美術位移)

    // 3. 任務(狀態)
    //發呆
    //追擊
    //攻擊
}
public abstract class AiState
{
    public DamageData getHit = null;
    protected Animator animator;
    protected Transform selfTransform;
    public AiState(Animator a, Transform self)
    {
        animator = a;
        selfTransform = self;
    }
    public abstract AiState SwitchState();
    public abstract void SetAnimation();
}
public class IdleState : AiState
{
    Transform target;
    bool findTarget;
    public IdleState(Transform t, PicoState state, Animator a, Transform self) : base(a, self)
    {
        target = t;
    }
    Vector3 originPosition;
    GameState switchStage = GameState.FirstStage;
    //Idle的啟動要是固定範圍 -- 要一直跟主角量距離
    //Idel 應該有個初始位置    
    public override AiState SwitchState()
    {
        var gameState = target.GetComponent<PicoState>().gameState;
        return gameState == switchStage ? new FightState(target, animator, selfTransform) : this;
    }

    public override void SetAnimation()
    {
        var ai = animator.GetCurrentAnimatorStateInfo(0);
        var percentage = ai.normalizedTime;
        if (percentage > 0.9f)
        {
            if (UnityEngine.Random.value >= 0.5f)
            {
                animator.SetTrigger("IdelSwitch");
            }
        }
    }
}
public class FightState : AiState
{
    //2. 可能會轉換
    Transform target;

    Vector3 direction;
    public FightState(Transform t, Animator a, Transform self) : base(a, self)
    {
        target = t;
        animator.SetBool("findTarget", true);
    }
    public override AiState SwitchState()
    {
        //0. 如果我被攻擊
        if (getHit != null) return new HurtState(animator, selfTransform, getHit);

        var distance = Vector3.Distance(target.position, selfTransform.position);
        int count = GetChasingNpcCount();
        if (distance <= 5 && UnityEngine.Random.value >= 0.75) return new AttackState(animator, selfTransform);
        if (distance > 5) return new ChaseState(target, animator, selfTransform);

        return this;
    }

    private int GetChasingNpcCount()
    {
        return ObjectManager.ChasingNpc.Count;
    }

    public override void SetAnimation()
    {
        //1. 總是面對主角
        //最大值 +-1 == 每次轉45度
        direction = target.position - selfTransform.position;
        var sign = Math.Sign(Vector3.Dot(direction, selfTransform.right));
        var degree = sign * Vector3.Angle(selfTransform.forward, direction);
        if (degree > 5 || degree < -5)
            selfTransform.Rotate(new Vector3(0, Math.Sign(degree), 0));

        if (UnityEngine.Random.value > 0.75)
        {
            animator.SetTrigger("taunt");
        }
    }
}
/// <summary>
/// 朝著角色方向移動
/// </summary>
public class ChaseState : AiState
{
    //要seek 遇到障礙物還要躲開
    Npc npc;
    Transform alertTarget;
    IKController iK;
    float attackRange = 5f;
    Vector3 direction;
    public ChaseState(Transform alertObject, Animator a, Transform self) : base(a, self)
    {
        alertTarget = alertObject;
        animator.SetBool("notReach", true);
        AddChasingNpc();

    }

    private void AddChasingNpc()
    {
        ObjectManager.ChasingNpc.Add(this);
    }

    private void RemoveChasingNpc()
    {
        ObjectManager.ChasingNpc.Remove(this);
    }

    public override AiState SwitchState()
    {

        //0. 如果我被攻擊
        if (getHit != null) return new HurtState(animator, selfTransform, getHit);


        //1. 如果目標物件消失於視野之外[，進行巡邏後]，回到發呆狀態

        //2. 如果目標物件進入攻擊範圍，則切換為攻擊模式
        var distance = Vector3.Distance(alertTarget.position, selfTransform.position);
        if (distance >= attackRange) return this;

        RemoveChasingNpc();
        if (distance < attackRange)
        {
            animator.SetBool("notReach", false);
            return new FightState(alertTarget, animator, selfTransform);
        }

        //3. 如果目標在追擊範圍內，則：(1) 如果追擊沒有滿，就進行追擊。(2) 若追擊已滿，就在外面咆哮。
        return new IdleState(alertTarget, selfTransform.GetComponent<PicoState>(), animator, selfTransform);
    }
    public void AroundOrClose()
    {

    }
    public void Turn() { }
    public void Seek() { }

    public override void SetAnimation()
    {
        direction = alertTarget.position - selfTransform.position;
        var sign = Math.Sign(Vector3.Dot(direction, selfTransform.right));
        var degree = sign * Vector3.Angle(selfTransform.forward, direction);
        if (degree > 5 || degree < -5)
            selfTransform.Rotate(new Vector3(0, Math.Sign(degree), 0));
    }
}

public class AttackState : AiState
{
    public AttackState(Animator a, Transform self) : base(a, self)
    {
    }

    // 1.等待(CD時間)
    // 2.攻擊
    public override AiState SwitchState()
    {
        //0. 如果我被攻擊
        if (getHit != null) return new HurtState(animator, selfTransform, getHit);

        return this;
    }

    public override void SetAnimation()
    {
      //  throw new NotImplementedException();
    }
}

public class HurtState : AiState
{
    float deadTime;
    DamageData damageData;
    Npc NpcData;
    public HurtState(Animator a, Transform self, DamageData d) : base(a, self)
    {
        NpcData = selfTransform.GetComponent<Npc>();
        damageData = d;
        animator.SetTrigger("getHit");
        DoOnce();
    }

    public override AiState SwitchState()
    {
        //NpcData = selfTransform.GetComponent<Npc>();
        if(NpcData.Hp > 0)
            return new FightState(damageData.Attacker, animator, selfTransform);
        //if (NpcData.Hp <= 0)
        //    return new HurtState(animator, selfTransform, getHit);

        return this;

        //判定動畫快播完時，下個動畫的銜接
        //回到 FightState
    }

    public override void SetAnimation()
    {
        if(NpcData.Hp <= 0)
        {
            deadTime += Time.deltaTime;
            if(getHit != null)
            {
                deadTime = 0f;
                animator.SetTrigger("toFlog");
                getHit = null;
                Debug.Log("Stop FLOGINGGGGG!!!!!!");
            }
        }
        else
        {
            deadTime = 0f;
        }
    }
    private void DoOnce()
    {
        // 依照 damageData.hit 決定播放哪個動畫。
        NpcData.Hp -= damageData.Damage;
        animator.SetFloat("hp", NpcData.Hp);
        //Debug.Log("789");
        if (damageData.Hit == HitType.light && NpcData.Hp > 0)
        {
            animator.SetTrigger("lightAttack");
            System.Random random = new System.Random();
            int type = random.Next(1, 3);
            Debug.Log(type);
            animator.SetInteger("playImpactType",type);
            //Debug.Log("456");
        }
        else //if(damageData.Hit == HitType.Heavy || NpcData.Hp <= 0)
        {
            animator.SetTrigger("heavyAttack");
            //Debug.Log("123");
        }

        if(NpcData.Hp < 0.0001f)
        {
            System.Random random = new System.Random();
            animator.SetInteger("playDeadType", random.Next(1, 3));
        }

    }
}






