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
        return target.GetComponent<PicoState>().gameState == switchStage ? new FightState(target, animator, selfTransform) : this;
    }

    public override void SetAnimation()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
        {
            if (UnityEngine.Random.value >= 0.5f)
                animator.SetTrigger("IdelSwitch");
        }
    }
}
public class FightState : AiState
{
    //1. 總是面對主角
    //2. 可能會轉換
    Transform target;
    public FightState(Transform t, Animator a, Transform self) : base(a, self)
    {
        target = t;
        target = animator.transform;
    }
    public override AiState SwitchState()
    {
        int count = GetChasingNpcCount();
        if (count > 30) return new ChaseState(target, animator, selfTransform);

        return this;
    }

    private int GetChasingNpcCount()
    {
        return ObjectManager.ChasingNpc.Count;
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
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
    float attackRange = 2f;
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
        RemoveChasingNpc();


        //1. 如果目標物件消失於視野之外[，進行巡邏後]，回到發呆狀態

        //2. 如果目標物件進入攻擊範圍，則切換為攻擊模式
        var distance = Vector3.Distance(alertTarget.position, selfTransform.position);
        if (distance <= attackRange) return new AttackState(animator, selfTransform);

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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }
}

public class HurtState : AiState
{
    DamageData damageData;
    public HurtState(Animator a, Transform self, DamageData d) : base(a, self)
    {
        damageData = d;
    }

    public override AiState SwitchState()
    {
        //判定動畫快播完時，下個動畫的銜接
        //回到 FightState
        throw new NotImplementedException();
    }

    public override void SetAnimation()
    {
        // 依照 damageData.hit 決定播放哪個動畫。
        throw new NotImplementedException();
    }
}


































/// <summary>
/// 面向玩家
/// </summary>
public class Alert : AiState
{
    //固定範圍內展開追擊 Physics.CheckSphere
    GameObject alertTarget;
    public Alert(GameObject alertObject)
    {
        alertTarget = alertObject;
    }
    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
public class Taunt : AiState
{
    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// 停下並站在角色面前
/// </summary>
public class Confrontation : AiState
{
    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
public class Stiff_1 : AiState
{
    AiState recoverState = new Confrontation();
    AiState stiff_2 = new Stiff_2();
    float recoverSeconds = 5;
    public bool hitAgain;
    public Stiff_1()
    {
    }
    public AiState Recover()
    {
        recoverSeconds -= Time.deltaTime;
        return recoverSeconds < 0 ? recoverState : this;
    }
    public void Beaten()
    {
        hitAgain = true;
    }

    public override AiState SwitchState()
    {
        if (hitAgain) return stiff_2;
        return Recover();
    }
}

internal class Stiff_2 : AiState
{
    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}

//用腳本的方式控制動作狀態？


//警戒巡邏 //地域性

//攻擊
//1. 腳色進入領地 2. Npc 看見腳色

//追進
//對峙
//逃開

//受攻僵直1
//受攻僵直2
//受攻僵直3
//受攻僵直4

//擊倒
//起身
