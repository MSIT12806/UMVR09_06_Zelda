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

    protected float armor;
    public float WeakTime = 3;//弱點持續時間
    public float ArmorBreakTime = 5; //破甲暈眩持續時間 
    public bool AttackFlaw = false;
    public DamageData GolemDamageData;
    public AiState(Animator a, Transform self)
    {
        animator = a;
        selfTransform = self;
    }
    public AiState(Animator a, Transform self,float armor)//菁英怪 & Boss 有盾值
    {
        animator = a;
        selfTransform = self;
        this.armor = armor;
    }

    public abstract AiState SwitchState();
    public abstract void SetAnimation();
}

#region Pico Machine

#endregion

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
        if (getHit != null) return new UsaoHurtState(animator, selfTransform, getHit, this);
        if (npc.Hp <= 0) return new UsaoDeathState(animator, selfTransform);

        var distance = Vector3.Distance(target.position, selfTransform.position);
        int count = GetChasingNpcCount();
        //if (distance <= 5 && UnityEngine.Random.value >= 0.75) return new AttackState(animator, selfTransform);
        if (distance > 5) return new UsaoChaseState(target, animator, selfTransform, this);

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
    AiState fightState;
    public UsaoChaseState(Transform alertObject, Animator a, Transform self, AiState fightState) : base(a, self)
    {
        alertTarget = alertObject;
        animator.SetBool("notReach", true);
        AddChasingNpc();
        this.fightState = fightState;
        npc = selfTransform.GetComponent<Npc>();
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
        if (getHit != null) return new UsaoHurtState(animator, selfTransform, getHit, fightState);


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

        var f = animator.GetFloat("forward");
        f = Math.Min(f + 0.02f, 1);
        animator.SetFloat("forward", f);
        if (npc.nextPosition != Vector3.zero)
            npc.nextPosition += direction * 0.001f;
    }

}

public class UsaoAttackState : AiState
{
    AiState fightState;
    public UsaoAttackState(Animator a, Transform self, AiState fightState) : base(a, self)
    {
        this.fightState = fightState;
    }

    // 1.等待(CD時間)
    // 2.攻擊
    public override AiState SwitchState()
    {
        //0. 如果我被攻擊
        if (getHit != null) return new UsaoHurtState(animator, selfTransform, getHit, fightState);

        return this;
    }

    public override void SetAnimation()
    {
        //  throw new NotImplementedException();
    }

}

public class UsaoHurtState : AiState
{
    AiState fightState;
    float deadTime;
    Npc npc;

    public UsaoHurtState(Animator a, Transform self, DamageData d, AiState fight) : base(a, self)
    {
        npc = selfTransform.GetComponent<Npc>();
        getHit = d;
        DoOnce();
        self.GetComponent<IKController>().LookAtObj = null;
        fightState = fight;
    }

    public override AiState SwitchState()
    {
        var anInfo = animator.GetCurrentAnimatorStateInfo(0);  //判定動畫快播完時，下個動畫的銜接
        if (npc.Hp > 0 && anInfo.normalizedTime > 0.9f)
            return fightState;        //回到 FightState
        if (npc.Hp <= 0)
            return new UsaoDeathState(animator, selfTransform);

        return this;

    }

    public override void SetAnimation()
    {
        if (npc.Hp <= 0)
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
        npc.Hp -= getHit.Damage;
        animator.SetFloat("hp", npc.Hp);
        if (getHit.Hit == HitType.light)
        {

            if (UnityEngine.Random.value >= 0.5f)
                animator.Play("GetHit.SwordAndShieldImpact02", 0);
            else
                animator.Play("GetHit.SwordAndShieldImpact01", 0);
            npc.nextPosition = selfTransform.position + getHit.Force;
            getHit = null;

            return;
        }
        if (getHit.Hit == HitType.Heavy)
        {
            animator.Play("GetHit.Flying Back Death", 0);
            animator.SetBool("Grounded", false);
            npc.grounded = false;
            npc.initVel = getHit.Force * 0.2f;
            npc.initVel.y = UnityEngine.Random.Range(0.3f, 0.8f);
        }

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
        var currentInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (currentInfo.IsName("GetHit.Flying Back Death")) return;
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
    float flyHpLimit;
    Npc npc;
    float attackWait = DragonStateCommon.RandonAttackScale();
    public DragonFightState(Transform t, Animator a, Transform self) : base(a, self)
    {
        target = t;
        head = self.FindAnyChild<Transform>("Head");
        flyHpLimit = 1000;
        npc = selfTransform.GetComponent<Npc>();
    }

    public override void SetAnimation()
    {
        DragonStateCommon.Stare(selfTransform, head, target);
    }

    public override AiState SwitchState()
    {

        if (npc.Hp < flyHpLimit)
        {
            return new DragonFlyState(animator, selfTransform);
        }
        if (attackWait > 0)
        {
            attackWait -= Time.deltaTime;
            return this;
        }
        // do attack
        var distance = Vector3.Distance(target.position, selfTransform.position);
        if (distance <= 3f)
        {
            return new DragonAttackState(target, "TailHit", animator, selfTransform);
        }
        if (distance <= 8f)
        {
            return new DragonAttackState(target, "FireHit", animator, selfTransform);
        }
        else
        {
            return new DragonChaseState(target, animator, selfTransform);
        }

    }
}
public class DragonFlyState : AiState
{
    public DragonFlyState(Animator a, Transform self) : base(a, self)
    {
        //不再受到任何攻擊，除非將其擊落(丟炸彈)
        animator.SetBool("Fly", true);
        //一直噴火球兒
    }

    public override void SetAnimation()
    {
    }

    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
public class DragonChaseState : AiState
{
    Transform target;
    public DragonChaseState(Transform t, Animator a, Transform self) : base(a, self)
    {
        target = t;
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
public class DragonAttackState : AiState
{
    bool attack;
    string triggerName;
    Transform target;
    public DragonAttackState(Transform transform, string trigger, Animator a, Transform self) : base(a, self)
    {
        this.triggerName = trigger;
        this.target = transform;
    }
    public override void SetAnimation()
    {
        animator.SetTrigger(triggerName);
        attack = true;
    }
    public override AiState SwitchState()
    {
        if (attack)
            return new DragonFightState(target, animator, selfTransform);


        return this;
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

    public static float RandonAttackScale()
    {
        return UnityEngine.Random.Range(3f, 10f);
    }
}
#endregion

#region Golem State Machine
public class GolemIdleState : AiState
{
    float attackDistance = 5f;
    Npc npcData;
    Transform target;
    public GolemIdleState(Transform t, Animator a, Transform self, float armor) : base(a, self)
    {
        target = t;
        npcData = selfTransform.GetComponent<Npc>();
    }
    public override void SetAnimation()
    {
        if(getHit != null)
        {
            npcData.Hp -= GolemDamageData.Damage/10;
            getHit = null;
        }
    }

    public override AiState SwitchState()
    {
        //切至Attack (追到後就打? 或亂數決定
        bool attack = false;
        if (attack)
        {
            animator.SetTrigger("Attack");
            return new GolemAttackState(target, animator, selfTransform, armor);
        }

        //切至Weak (攻擊後就露出? 或亂數決定
        bool weak = false;
        if (weak)
        {
            animator.SetBool("ShowWeakness", true);
            return new GolemWeakState(target, animator, selfTransform, armor, WeakTime);
        }
        //切至Skill (血量到特定%? 或亂數決定
        bool skill = false;
        if (skill)
        {
            animator.SetTrigger("Skill");
            return new GolemSkillState(target, animator, selfTransform);
        }
        //切至Chase (距離玩家 > 攻擊範圍
        if( (target.position-selfTransform.position).magnitude > attackDistance)
        {
            animator.SetBool("notReach", true);
            return new GolemChaseState(target, animator, selfTransform);
        }
        //切至Roar (血量低於50%
        if(npcData.Hp < 100)
        {
            animator.SetTrigger("SetShield");
            return new GolemRoarState(target, animator, selfTransform);
        }
        throw new NotImplementedException();
    }
}
public class GolemChaseState : AiState
{
    Transform target;
    Npc npcData;
    float attackDistance = 5f;
    public GolemChaseState(Transform t, Animator a, Transform self) : base(a, self)
    {
        npcData = selfTransform.GetComponent<Npc>();
        target = t;
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

    public override void SetAnimation()
    {
        if (getHit != null)
        {
            npcData.Hp -= GolemDamageData.Damage / 10;
            getHit = null;
        }
    }

    public override AiState SwitchState()
    {
        //到玩家旁邊切回idle
        float distance = (selfTransform.position - target.position).magnitude;
        if (distance <= attackDistance)
        {
            RemoveChasingNpc();
            animator.SetBool("notReach", false);
            return new GolemIdleState(target, animator, selfTransform,armor);
        }
        else if(distance > attackDistance)
        {
            return this;
        }

        throw new NotImplementedException();
    }
}

public class GolemWeakState : AiState
{
    Npc npcData;
    Transform target;
    float showWeaknessTime;
    public GolemWeakState(Transform t, Animator a, Transform self, float armor, float weakTime) : base(a, self, armor)
    {
        npcData = selfTransform.GetComponent<Npc>();
        target = t;
        showWeaknessTime = 0;
    }
    public override void SetAnimation()
    {
        showWeaknessTime += Time.deltaTime;
        animator.SetBool("ShowWeakness", true);
        //animator.SetFloat("WeakTime", showWeaknessTime);


        if (getHit != null)
        {
            npcData.Hp -= GolemDamageData.Damage / 10;
            armor -= 1;
            getHit = null;
        }
    }

    public override AiState SwitchState()
    {
        //露出時間結束 切回idle
        if(showWeaknessTime > WeakTime)
        {
            animator.SetBool("ShowWeakness", false);
            return new GolemIdleState(target, animator, selfTransform, armor);
        }
        //Armor被擊破 切至ArmorBreak
        if (armor <= 0)
        {
            animator.SetTrigger("ArmorBreak");
            return new GolemArmorBreakState(target, animator, selfTransform);
        }
        //
        else return this;
    }

}

public class GolemArmorBreakState : AiState
{
    float armorValue = 6;
    Transform target;
    float time;

    Npc npcData;
    public GolemArmorBreakState(Transform t, Animator a, Transform self) : base(a, self)
    {
        target = t;
        time = 0;
        npcData = selfTransform.GetComponent<Npc>();
    }

    public override void SetAnimation()
    {
        time += Time.deltaTime;
        if(getHit != null)
        {
            animator.SetTrigger("getHit");
            npcData.Hp -= GolemDamageData.Damage;
            getHit = null;
        }

    }

    public override AiState SwitchState()
    {
        //暈眩時間結束 切回idle
        //Armor補滿
        if(time > 5)
        {
            animator.SetTrigger("ArmorRecover");
            return new GolemIdleState(target, animator, selfTransform, armorValue);
        }
        if (time <= 5)
        {
            return this;
        }
        throw new NotImplementedException();
    }
}
public class GolemAttackState : AiState
{
    Transform target;
    Npc npcData;
    public GolemAttackState(Transform t, Animator a, Transform self, float armor) : base(a, self, armor)
    {
        target = t;
        npcData = selfTransform.GetComponent<Npc>();
    }

    public override void SetAnimation()
    {
        if (getHit != null)
        {
            npcData.Hp -= GolemDamageData.Damage / 10;
            getHit = null;
        }
    }

    public override AiState SwitchState()
    {
        //被完美閃避 短暫露出Armor Armor被擊破 切至ArmorBreak
        if (AttackFlaw)
        {
            animator.SetTrigger("ArmorBreak");
            return new GolemArmorBreakState(target, animator, selfTransform);
        }
        //攻擊結束 切回idle
        bool finish = false;
        if (finish)
        {
            return new GolemIdleState(target, animator, selfTransform, armor);
        }
        throw new NotImplementedException();
    }
}
public class GolemSkillState : AiState
{
    Transform target;
    Npc npcData;
    public GolemSkillState(Transform t, Animator a, Transform self) : base(a, self)
    {
        npcData = selfTransform.GetComponent<Npc>();
        target = t;
    }

    public override void SetAnimation()
    {
        if (getHit != null)
        {
            //待修改
            if(GolemDamageData.Hit == HitType.Ice)
            {
                AttackFlaw = true;
            }

            npcData.Hp -= GolemDamageData.Damage / 10;
            getHit = null;
        }
    }

    public override AiState SwitchState()
    {
        //技能施放結束 切回idle
        bool finish = false;
        if (finish)
        {
            return new GolemIdleState(target, animator, selfTransform, armor);
        }

        //玩家利用西卡之石破解技能 切至ArmorBreak
        if (AttackFlaw)
        {
            animator.SetTrigger("SheikahDefense");
            return new GolemArmorBreakState(target, animator, selfTransform);
        }
        throw new NotImplementedException();
    }
}
public class GolemRoarState : AiState
{
    Transform target;
    public GolemRoarState(Transform t, Animator a, Transform self) : base(a, self)
    {
        target = t;
    }

    public override void SetAnimation()
    {
        if (getHit != null)
        {
            getHit = null;
        }
    }

    public override AiState SwitchState()
    {
        //施放完 切至idle
        bool finish = false;
        if (finish)
        {
            return new GolemIdleState(target, animator, selfTransform, armor);
        }
        throw new NotImplementedException();
    }
}



#endregion