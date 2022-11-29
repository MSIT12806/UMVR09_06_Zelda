using CombatSystem;
using Ron;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.GraphicsBuffer;

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

#region Usao State Machine
public class UsaoIdleState : AiState
{
    Transform target;
    public UsaoIdleState(Transform t, PicoState state, Animator a, Transform self) : base(a, self)
    {
        target = t;
    }
    GameState switchStage = GameState.FirstStage;
    //Idle的啟動要是固定範圍 -- 要一直跟主角量距離
    //Idel 應該有個初始位置    
    public override AiState SwitchState()
    {
        var gameState = target.GetComponent<PicoState>().gameState;
        return gameState == switchStage ? new UsaoFightState(target, animator, selfTransform) : this;
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
public class UsaoFightState : AiState
{
    //2. 可能會轉換
    Transform target;

    Vector3 direction;
    public UsaoFightState(Transform t, Animator a, Transform self) : base(a, self)
    {
        target = t;
        animator.SetBool("findTarget", true);
        self.GetComponent<IKController>().LookAtObj = target;
    }
    public override AiState SwitchState()
    {
        //0. 如果我被攻擊
        var npc = selfTransform.GetComponent<Npc>();
        if (npc.Hp <= 0) return new UsaoDeathState(animator, selfTransform);
        if (getHit != null) return new UsaoHurtState(animator, selfTransform, getHit);

        var distance = Vector3.Distance(target.position, selfTransform.position);
        int count = GetChasingNpcCount();
        //if (distance <= 5 && UnityEngine.Random.value >= 0.75) return new AttackState(animator, selfTransform);
        //if (distance > 5) return new ChaseState(target, animator, selfTransform);

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
        Taunt();

    }

    private void Taunt()
    {
        var aniInfo = animator.GetCurrentAnimatorStateInfo(0);
        var r = UnityEngine.Random.value;
        if (aniInfo.IsName("Fight") && aniInfo.normalizedTime >= 0.9 && r > 0.99)
        {
            animator.SetTrigger("taunt");
        }
    }

}
/// <summary>
/// 朝著角色方向移動
/// </summary>
public class UsaoChaseState : AiState
{
    //要seek 遇到障礙物還要躲開
    Npc npc;
    Transform alertTarget;
    float attackRange = 5f;
    Vector3 direction;
    public UsaoChaseState(Transform alertObject, Animator a, Transform self) : base(a, self)
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
        if (getHit != null) return new UsaoHurtState(animator, selfTransform, getHit);


        //1. 如果目標物件消失於視野之外[，進行巡邏後]，回到發呆狀態

        //2. 如果目標物件進入攻擊範圍，則切換為攻擊模式
        var distance = Vector3.Distance(alertTarget.position, selfTransform.position);
        if (distance >= attackRange) return this;

        RemoveChasingNpc();
        if (distance < attackRange)
        {
            animator.SetBool("notReach", false);
            return new UsaoFightState(alertTarget, animator, selfTransform);
        }

        //3. 如果目標在追擊範圍內，則：(1) 如果追擊沒有滿，就進行追擊。(2) 若追擊已滿，就在外面咆哮。
        return new UsaoIdleState(alertTarget, selfTransform.GetComponent<PicoState>(), animator, selfTransform);
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

public class UsaoAttackState : AiState
{
    public UsaoAttackState(Animator a, Transform self) : base(a, self)
    {
    }

    // 1.等待(CD時間)
    // 2.攻擊
    public override AiState SwitchState()
    {
        //0. 如果我被攻擊
        if (getHit != null) return new UsaoHurtState(animator, selfTransform, getHit);

        return this;
    }

    public override void SetAnimation()
    {
        //  throw new NotImplementedException();
    }

}

public class UsaoHurtState : AiState
{
    Transform target;
    float deadTime;
    Npc NpcData;
    public UsaoHurtState(Animator a, Transform self, DamageData d) : base(a, self)
    {
        NpcData = selfTransform.GetComponent<Npc>();
        getHit = d;
        target = d.Attacker;
        DoOnce();
        self.GetComponent<IKController>().LookAtObj = null;
    }

    public override AiState SwitchState()
    {
        var anInfo = animator.GetCurrentAnimatorStateInfo(0);  //判定動畫快播完時，下個動畫的銜接
        if (NpcData.Hp > 0 && anInfo.normalizedTime > 0.9f)
            return new UsaoFightState(target, animator, selfTransform);        //回到 FightState
        if (NpcData.Hp <= 0)
            return new UsaoDeathState(animator, selfTransform);

        return this;

    }

    public override void SetAnimation()
    {
        if (NpcData.Hp <= 0)
        {
            deadTime += Time.deltaTime;
            if (getHit != null)
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
        getHit = null;
    }
    private void DoOnce()
    {
        // 依照 damageData.hit 決定播放哪個動畫。
        NpcData.Hp -= getHit.Damage;
        animator.SetFloat("hp", NpcData.Hp);
        if (getHit.Hit == HitType.light && NpcData.Hp > 0)
        {
            //animator.SetTrigger("lightAttack");
            ////System.Random random = new System.Random();
            ////int type = random.Next(1, 3);
            //animator.SetInteger("playImpactType", 2);//暫時廢棄 1 的動作

            if (UnityEngine.Random.value >= 0.5f)
                animator.Play("GetHit.SwordAndShieldImpact02", 0);
            else
                animator.Play("GetHit.SwordAndShieldImpact01", 0);
            NpcData.nextPosition = selfTransform.position - (getHit.Attacker.position - selfTransform.position).normalized * 0.5f;
            getHit = null;

            return;
        }
        if (getHit.Hit == HitType.Heavy && NpcData.Hp > 0)
        {
            animator.Play("GetHit.Flying Back Death", 0);
            NpcData.nextPosition = selfTransform.position - (getHit.Attacker.position - selfTransform.position).normalized * 1f;
            getHit = null;
            return;
        }

        ////死亡
        //if (NpcData.Hp < 0.0001f)
        //{
        //    System.Random random = new System.Random();
        //    animator.SetInteger("playDeadType", random.Next(1, 3));
        //    getHit = null;
        //}
    }

}

public class UsaoDeathState : AiState
{
    int deathTime;
    public UsaoDeathState(Animator a, Transform self) : base(a, self)
    {
        if (UnityEngine.Random.value >= 0.5f)
            a.Play("GetHit.Standing React Death Right");
        else
            a.Play("GetHit.Standing React Death Left");
        deathTime = Time.frameCount;
    }

    public override void SetAnimation()
    {
        if (Time.frameCount > deathTime + 60)
        {
            selfTransform.gameObject.SetActive(false);
        }
    }

    public override AiState SwitchState()
    {
        return this;
    }
}
#endregion

#region Dragon State Machine

public class DragonIdleState : AiState
{
    Transform target;
    public DragonIdleState(Transform t, Animator a, Transform self) : base(a, self)
    {
        target = t;
    }

    public override void SetAnimation()
    {
    }

    public override AiState SwitchState()
    {
        //如果pico 走進該區域， return new DragonFightState();
        var stage = target.GetComponent<PicoState>();

        if ((int)stage.gameState == 3)
            return new DragonFightState(target, animator, selfTransform);


        return this;
    }
}

public class DragonFightState : AiState
{
    Transform target;
    Transform head;
    public DragonFightState(Transform t, Animator a, Transform self) : base(a, self)
    {
        target = t;
        head = self.FindAnyChild<Transform>("Head");
    }

    public override void SetAnimation()
    {
        DragonStateCommon.Stare(selfTransform, head, target);
    }

    public override AiState SwitchState()
    {
        //1. 距離較遠 -> 吐火球  Fireball Shoot
        //2. 距離較近 -> 鋼鐵尾巴 
        //3. 距離太遠 -> return new DragonChaseState();
        //4. 血量低於某數值以下 -> 起飛
        throw new NotImplementedException();
    }
}
public class DragonFlyState : AiState
{
    public DragonFlyState(Animator a, Transform self) : base(a, self)
    {
        //不再受到任何攻擊，除非將其擊落(丟炸彈)
        //一直噴火球兒
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
public class DragonChaseState : AiState
{
    public DragonChaseState(Animator a, Transform self) : base(a, self)
    {
    }

    public override void SetAnimation()
    {
        //面向 pico
        //朝著 pico 用 aStar 做 seek ，直到攻擊範圍內
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
public class DragonFireBallAttackState : AiState
{
    public DragonFireBallAttackState(Animator a, Transform self) : base(a, self)
    {
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
public class DragonTailAttackState : AiState
{
    public DragonTailAttackState(Animator a, Transform self) : base(a, self)
    {
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
public class DragonDizzyState : AiState
{
    public DragonDizzyState(Animator a, Transform self) : base(a, self)
    {
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
public class DragonDeathState : AiState
{
    public DragonDeathState(Animator a, Transform self) : base(a, self)
    {
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
public static class DragonStateCommon
{
    public static void Stare(Transform selfBody, Transform selfHead, Transform target)
    {
        //1. 身體面對對方
        var bodyFaceDirection = target.position;
        bodyFaceDirection.y = selfBody.position.y;
        selfBody.LookAt(bodyFaceDirection.WithoutY(0.75f));
        //2. 看向對方
        selfHead.LookAt(target);
    }
}
#endregion



