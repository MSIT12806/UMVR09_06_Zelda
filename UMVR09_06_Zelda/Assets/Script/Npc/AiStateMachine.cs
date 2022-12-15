using CombatSystem;
using Ron;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public abstract class AiState
{
    public DamageData getHit = null;
    public readonly string Name;
    protected Animator animator;
    protected Transform selfTransform;
    protected NpcHelper npcHelper;
    protected Npc npc;
    protected PicoState picoState;

    public AiState(Animator a, Transform self, NpcHelper nh, string name, PicoState state)
    {
        animator = a;
        selfTransform = self;
        npcHelper = nh;
        Name = name;
        picoState = state;
        npc = self.GetComponent<Npc>();
    }


    public abstract AiState SwitchState();
    public abstract void SetAnimation();
}

#region Common State Machine
//public class IdelState : AiState
//{
//    public IdelStateInfo SetAnimations;
//    int randomAnimation;
//    //switch state
//    ////切到 fight

//    //SetAnimation
//    ////當動作播完  幾種動作的切換  還是要每偵都要檢查捏  不然寫個事件好惹

//還是會有一個兜底的動作，其他的動作用 trigger觸發
//}
//public class FightState : AiState
//{
//    //可以有很多種攻擊方式
//    //但是只有一種追擊型態
//    //透過重新更新fightstate 來轉換追擊型態 => RefreshFightState
//    public AttackStateInfo[] attackStateInfos;

//    //發呆時間

//    //switch state
//    ////是否丟失目標
//    //////切到Idle
//    ////getHit != null
//    //////切到受傷

//    ////是否超過發呆時間
//    //////返回 fight

//    ////距離判定
//    //////切到attack
//    /////*切到chase

//    ////目標死亡 or 消失
//    //////切到 chase



//    //SetAnimation
//    ////面向目標

//    // private RefreshDazeTime()
//}
//public class AttackState : AiState
//{
//    AttackStateInfo attackStateInfo;
//    FightState fightState;
//    //建構子
//    //// 發動攻擊  setTrigger

//    //switch state
//    ////getHit != null
//    //////切到受傷
//    ////返回 fight(更新發呆時間)
//}
//public class ChaseState : AiState
//{

//    //建構子
//    ////朝著npcHelper.target移動
//    /// //setBoolean

//    //switch state

//    //setAnimation
//    //// 更新目標位置 1. npcHelper.originPosition; 2. targetPosition; 3. 

//}

//public class HurtState : AiState
//{
//    FightState fightState;
//    //建構子
//    //// 取得 damagedata
//    //// 扣血

//    //set animation
//    ////還是要判斷現在播動畫到幾成?
//    ////播放受傷動畫、擊退位移

//    //switch state
//    //// hp>=0
//    //////返回 fight
//    /////*死亡
//}
//public class DeathState : AiState
//{
//    public float deathTime;
//    public Action dosomething;
//    //物件消失
//}
#endregion

#region Pico Machine

#endregion

#region Usao State Machine
public abstract class UsaoAiState : AiState
{
    protected UsaoManager usaoManager;
    protected UsaoAiState(Animator a, Transform self, NpcHelper nh, string name, PicoState state) : base(a, self, nh, name, state)
    {
        usaoManager = (UsaoManager)nh;
    }
}
public class UsaoIdleState : UsaoAiState
{
    Transform target;
    UsaoManager manager;

    public UsaoIdleState(Transform t, PicoState state, Animator a, Transform self, NpcHelper nh) : base(a, self, nh, "Idle", t.GetComponent<PicoState>())
    {
        target = t;
        manager = (UsaoManager)nh;
    }
    //Idle的啟動要是固定範圍 -- 要一直跟主角量距離
    //Idel 應該有個初始位置    
    public override AiState SwitchState()
    {
        return picoState.gameState == npc.gameState ? new UsaoFightState(target, animator, selfTransform, npcHelper) : this;
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
public class UsaoFightState : UsaoAiState
{
    //2. 可能會轉換
    Transform target;
    Transform head;
    Vector3 direction;
    float dazeSeconds;
    float keepDistance = 10f;
    float attackDistance = 2f;
    float keepOrAttack;
    public UsaoFightState(Transform t, Animator a, Transform self, NpcHelper nh) : base(a, self, nh, "Fight", t.GetComponent<PicoState>())
    {
        target = t;
        animator.SetBool("findTarget", true);
        head = selfTransform.FindAnyChild<Transform>("Character1_Head");
        RefreshDazeTime();
        keepOrAttack = UnityEngine.Random.value;
    }
    public override AiState SwitchState()
    {
        //如果我被攻擊
        if (getHit != null) return new UsaoHurtState(animator, selfTransform, getHit, this, npcHelper);

        dazeSeconds -= Time.deltaTime;
        if (dazeSeconds > 0) return this;


        var distance = Vector3.Distance(target.position, selfTransform.position);
        int count = GetChasingNpcCount();
        if (keepOrAttack > 0.3)
        {
            if (distance > keepDistance) return new UsaoChaseState(target, animator, selfTransform, this, npcHelper);
            else if (distance <= keepDistance) return new UsaoAttackState(animator, selfTransform, this, npcHelper);
        }
        else
        {
            if (distance > attackDistance) return new UsaoChaseState(target, animator, selfTransform, this, npcHelper);
            else if (distance <= attackDistance) return new UsaoAttackState(animator, selfTransform, this, npcHelper);
        }

        RefreshDazeTime();
        return this;
    }

    public void RefreshDazeTime()
    {
        dazeSeconds = UnityEngine.Random.Range(1, 5);
    }

    private int GetChasingNpcCount()
    {
        return ObjectManager.ChasingNpc.Count;
    }

    public override void SetAnimation()
    {
        //1. 總是面對主角


        //AiStateCommon.Look(head, ObjectManager.MainCharacterHead);

        TauntRandomly();

    }

    private void TauntRandomly()
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
public class UsaoChaseState : UsaoAiState
{
    //要seek 遇到障礙物還要躲開
    Npc npc;
    Transform alertTarget;
    float attackRange = 10f;
    Vector3 direction;
    UsaoFightState fightState;
    public UsaoChaseState(Transform alertObject, Animator a, Transform self, UsaoFightState fightState, NpcHelper nh) : base(a, self, nh, "Chase", alertObject.GetComponent<PicoState>())
    {
        var r = UnityEngine.Random.value;
        attackRange = r > 0.3 ? 10f : 3f;
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
        if (getHit != null) return new UsaoHurtState(animator, selfTransform, getHit, fightState, npcHelper);


        //1. 如果目標物件消失於視野之外[，進行巡邏後]，回到發呆狀態
        if (usaoManager.stage != picoState.gameState)
        {
            animator.SetBool("notReach", false);
            return new UsaoIdleState(alertTarget, picoState, animator, selfTransform, usaoManager);
        }
        //2. 如果目標物件進入攻擊範圍，則切換為攻擊模式
        var distance = Vector3.Distance(alertTarget.position, selfTransform.position);
        if (distance >= attackRange) return this;

        RemoveChasingNpc();
        if (distance < attackRange)
        {
            animator.SetBool("notReach", false);
            return new UsaoFightState(alertTarget, animator, selfTransform, npcHelper);
        }

        //3. 如果目標在追擊範圍內，則：(1) 如果追擊沒有滿，就進行追擊。(2) 若追擊已滿，就在外面咆哮。
        return new UsaoIdleState(alertTarget, selfTransform.GetComponent<PicoState>(), animator, selfTransform, npcHelper);
    }

    public override void SetAnimation()
    {
        npcHelper.Turn(alertTarget.position - selfTransform.position);
        var f = animator.GetFloat("forward");
        f = Math.Min(f + 0.02f, 1);
        animator.SetFloat("forward", f);
        if (npc.nextPosition != Vector3.zero)
            npc.nextPosition += direction * 0.001f;
    }

}

public class UsaoAttackState : UsaoAiState
{
    UsaoFightState fightState;
    public UsaoAttackState(Animator a, Transform self, UsaoFightState fightState, NpcHelper nh) : base(a, self, nh, "Attack", null)
    {
        this.fightState = fightState;
    }

    // 1.等待(CD時間)
    // 2.攻擊
    public override AiState SwitchState()
    {
        //0. 如果我被攻擊
        if (getHit != null) return new UsaoHurtState(animator, selfTransform, getHit, fightState, npcHelper);

        fightState.RefreshDazeTime();
        return fightState;
    }

    public override void SetAnimation()
    {
        animator.SetTrigger("attack");
        animator.SetInteger("attackWay", UnityEngine.Random.Range(0, 7));

        //攻擊判定交給動作事件處理
        //NpcCommon.AttackDetection(selfTransform.position, selfTransform.forward, 5f, 2f, false, new DamageData(5, Vector3.zero, HitType.light), "Player");
    }

}

public class UsaoHurtState : UsaoAiState
{
    UsaoFightState fightState;
    float deadTime;
    Npc npc;
    public UsaoHurtState(Animator a, Transform self, DamageData d, UsaoFightState fight, NpcHelper nh) : base(a, self, nh, "Hurt", null)
    {
        npc = selfTransform.GetComponent<Npc>();
        getHit = d;
        DoOnce();
        fightState = fight;
    }

    public override AiState SwitchState()
    {
        var anInfo = animator.GetCurrentAnimatorStateInfo(0);  //判定動畫快播完時，下個動畫的銜接
        if (npc.Hp > 0 && anInfo.normalizedTime > 0.9f)
        {
            fightState.RefreshDazeTime();
            return fightState;
        }
        if (npc.Hp <= 0)
        {
            return new UsaoDeathState(animator, selfTransform, npcHelper, getHit);
        }

        getHit = null;
        return this;

    }

    public override void SetAnimation()
    {

    }
    private void DoOnce()
    {
        // 依照 damageData.hit 決定播放哪個動畫。
        npc.Hp -= getHit.Damage;
        animator.SetFloat("hp", npc.Hp);
        if (getHit.Hit == HitType.light)
        {

            if (UnityEngine.Random.value >= 0.5f)
                npc.PlayAnimation("GetHit.SwordAndShieldImpact02");
            else
                npc.PlayAnimation("GetHit.SwordAndShieldImpact01");
            if (npc.collide == false)
            {
                //npc.nextPosition = selfTransform.position + getHit.Force;//不知道為什麼  註解掉之後還是會擊飛
            }

            return;
        }
        if (getHit.Hit == HitType.Heavy)
        {

            if (UnityEngine.Random.value >= 0.5)
            {
                animator.CrossFade("GetHit.Die01_SwordAndShield", 0.2f, 0);
            }
            else
            {
                npc.PlayAnimation("GetHit.Flying Back Death");
                getHit.Force.y = 0.75f;
            }
            npc.KnockOff(getHit.Force);
        }

        //if (NpcData.Hp < 0.0001f)
        //{
        //    System.Random random = new System.Random();
        //    animator.SetInteger("playDeadType", random.Next(1, 3));
        //    getHit = null;
        //}
    }

}

public class UsaoDeathState : UsaoAiState
{
    int deathTime;
    public UsaoDeathState(Animator a, Transform self, NpcHelper nh, DamageData damageData) : base(a, self, nh, "Death", null)
    {

        deathTime = Time.frameCount;
        if (damageData.Hit == HitType.Heavy) return;
        if (UnityEngine.Random.value >= 0.5f)
            npc.PlayAnimation("GetHit.Standing React Death Right");
        else
            npc.PlayAnimation("GetHit.Standing React Death Left");

    }

    public override void SetAnimation()
    {
        if (Time.frameCount > deathTime + 180)
        {
            //死亡程序
            var fxGo = ObjectManager.DieFx.Dequeue();
            fxGo.transform.position = selfTransform.position;

            //移出場外以免打擊判定與推擠判定
            selfTransform.position.AddY(-1000);

            //移出活人池增益效能
            ObjectManager.NpcsAlive.Remove(selfTransform.gameObject.GetInstanceID()); 

            //移入備用池
            ObjectManager.StageDeathPool[(int)npc.gameState].Add(selfTransform.gameObject.GetInstanceID(), selfTransform.gameObject);

            //怪物數量監控
            ObjectManager.StageMonsterMonitor[(int)npc.gameState]--;

            //死亡消失與特效
            selfTransform.gameObject.SetActive(false);
            fxGo.SetActive(true);
            ObjectManager.DieFx.Enqueue(fxGo);
        }
    }

    public override AiState SwitchState()
    {
        return this;
    }
}
#endregion


/// <summary>
/// 可以說是 飛翔狀態的 FightState
/// </summary>
public static class AiStateCommon
{

    //其實不應該寫在這邊
    public static bool Turn(Transform body, Vector3 direction)
    {
        var degree = Vector3.SignedAngle(body.forward.WithY(), direction.WithY(), Vector3.up);
        if (degree < -4)
        {
            body.Rotate(Vector3.up, -2);
            return true;

        }
        else if (degree > 4)
        {
            body.Rotate(Vector3.up, 2);
            return true;
        }

        return false;
    }

    public static void Look(Transform head, Transform targetHead)
    {
        bool over = false;
        var degreeY = Vector3.Angle(head.forward.WithY(), (targetHead.position - head.position).WithY());
        degreeY = degreeY * Mathf.Sign(Vector3.SignedAngle(head.forward, targetHead.position - head.position, Vector3.up));
        if (degreeY > 80)
        {
            degreeY = 80;
            over = true;
        }
        else if (degreeY < -80)
        {
            degreeY = -80;
            over = true;
        }
        var degreeX = Vector3.Angle((targetHead.position - head.position).WithY(), (targetHead.position - head.position));
        degreeX = degreeX * Mathf.Sign(Vector3.SignedAngle((targetHead.position - head.position).WithY(), targetHead.position - head.position, -head.right));
        if (degreeX > 40)
        {
            degreeX = 40;
            over = true;
        }
        else if (degreeX < -40)
        {
            degreeX = -40;
            over = true;
        }
        var d = head.forward.WithY();
        d = Quaternion.AngleAxis(degreeY, Vector3.up) * d;
        d = Quaternion.AngleAxis(degreeX, d.GetLocalRight()) * d;
        head.forward = d.normalized;
        if (!over)
        {
            head.LookAt(targetHead);
        }
    }
    public static float RandonAttackScale()
    {
        return UnityEngine.Random.Range(3f, 10f);
    }

    public static void LookAtByIk(IKController selfIk, Transform targetHead)
    {
        selfIk.LookAtObj = targetHead;
    }
}

#region Golem State Machine
public static class Once
{
    public static float IceDestroyTime = 3.5f;//冰塊在場上持續時間
    public static bool CanSetShield = true;
    public static bool CanMove = true;
    public static Vector3 IcePosision;
}
public abstract class GolemBaseState : AiState
{

    protected Npc npcData;
    protected float max_armor = 10;
    protected float armor;
    protected float nowArmor;
    public float WeakTime = 4;//弱點持續時間
    public float ArmorBreakTime = 5; //破甲暈眩持續時間 
    public bool AttackFlaw = false;
    public DamageData GolemDamageData;

    public float FreezeTime = 0f;
    public GolemBaseState(Animator a, Transform self, NpcHelper nh, float ar) : base(a, self, nh, "", null)//菁英怪 & Boss 有盾值
    {
        animator = a;
        selfTransform = self;
        //armor = max_armor;
        npcData = selfTransform.GetComponent<Npc>();
        nowArmor = ar;
    }

    public float GetArmor()
    {
        return nowArmor;
    }
}
public class GolemIdleState : GolemBaseState
{
    float attackDistance = 4.5f;
    Transform target;
    PicoState picoState;
    bool goWeakState = false;
    GolemManager gm;
    public GolemIdleState(Transform t, Animator a, Transform self, float armor, NpcHelper nh) : base(a, self, nh, armor)
    {
        animator.ResetTrigger("Attack01");
        animator.ResetTrigger("Attack02");
        animator.ResetTrigger("Attack03");
        animator.ResetTrigger("Skill");
        animator.ResetTrigger("SheikahDefense");
        animator.ResetTrigger("getHit");
        animator.ResetTrigger("ArmorBreak");
        animator.ResetTrigger("SetShield");
        animator.ResetTrigger("FeverAttack");
        animator.ResetTrigger("Skill2");
        target = t;
        nowArmor = armor;
        picoState = target.GetComponent<PicoState>();
        //npcData = selfTransform.GetComponent<Npc>();
        gm = (GolemManager)npcHelper;
    }
    public override void SetAnimation()
    {
        //FreezeTime -= Time.deltaTime;

        if (false)//第一波小怪殺完
        {
            animator.SetFloat("StandSpeed", 0.5f);
        }

        //Debug.Log("111111");
        if (getHit != null)
        {
            Debug.Log($"before {npcData.Hp}");
            GolemManager gm = (GolemManager)npcHelper;
            if(gm.Shield <= 0)
            {
                npcData.Hp -= getHit.Damage;
            }
            if (getHit.DamageState.damageState == DamageState.Fever)
            {
                goWeakState = true;
                gm.Shield -= 5;
            }
            getHit = null;
            
        }
    }

    public override AiState SwitchState()
    {
        //切至Dead (血量歸0
        if (npcData.Hp < 0.0001f)
        {
            animator.SetTrigger("Dead");
            return new GolemDeadState(target, animator, selfTransform, npcHelper);
        }
        if (picoState.gameState != GameState.ThridStage)
            return this;
        //切至Roar (血量低於50% //do once
        if (npcData.Hp <= npcHelper.MaxHp / 2 && Once.CanSetShield)
        {
            Once.CanSetShield = false;
            animator.SetTrigger("SetShield");
            return new GolemRoarState(target, animator, selfTransform, nowArmor, npcHelper);
        }

        //被無雙技打

        if (goWeakState && gm.Shield <= 0 )
        {
            goWeakState = false;
            animator.SetTrigger("FeverAttack");
            return new GolemWeakState(target, animator, selfTransform, nowArmor, npcHelper);
        }

        float distance = (target.position - selfTransform.position).magnitude;

        //切至Skill (血量到特定%? 或亂數決定

        System.Random random = new System.Random();
        //int rnd = random.Next(1, 3);//判斷要不要用技能


        //if (rnd < 3 && distance <= 15)
        //{
        //    //animator.SetTrigger("Skill");
        //}

        //切至Attack (追到後就打? 或亂數決定
        if (distance <= attackDistance)
        {
            int rnd = random.Next(1, 4);
            if (rnd < 3)
                return new GolemAttackState(target, animator, selfTransform, nowArmor, npcHelper);
            if(rnd == 3)
                return new GolemSkillState(target, animator, selfTransform, nowArmor, npcHelper);

        }

        //切至Chase (距離玩家 > 攻擊範圍
        if (distance > attackDistance)
        {
            //Debug.Log((target.position - selfTransform.position).magnitude);
            animator.SetBool("NotReach", true);
            return new GolemChaseState(target, animator, selfTransform, npcHelper, nowArmor);
        }



        return this;
    }
}
public class GolemChaseState : GolemBaseState
{
    Transform target;
    float attackDistance = 4.5f;
    AnimatorStateInfo currentAnimation;
    private bool goWeakState;

    GolemManager gm;

    public GolemChaseState(Transform t, Animator a, Transform self, NpcHelper nh, float armor) : base(a, self, nh, armor)
    {
        target = t;
        nowArmor = armor;
        AddChasingNpc();
        gm = (GolemManager)npcHelper;
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

        FreezeTime -= Time.deltaTime;
        if(FreezeTime <= 0)
            LookAt();

        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);
        if (!animator.IsInTransition(0) && currentAnimation.IsName("Walk"))
            selfTransform.Translate(0, 0, 0.05f);
        if (getHit != null)
        {
            if(getHit.DamageState.damageState == DamageState.TimePause)
            {
                FreezeTime = 5;
            }

            GolemManager gm = (GolemManager)npcHelper;
            if (gm.Shield <= 0)
            {
                npcData.Hp -= getHit.Damage;
            }
            if (getHit.DamageState.damageState == DamageState.Fever)
            {
                goWeakState = true;
                gm.Shield -= 5;
            }
            getHit = null;
        }
    }
    public void LookAt()
    {
        Vector3 dir = target.position - selfTransform.position;
        dir.y = 0;
        dir.Normalize();
        float dot = Vector3.Dot(selfTransform.forward, dir);

        if (dot > 1) dot = 1;
        else if (dot < -1) dot = -1;

        float radian = Mathf.Acos(dot);
        float degree = radian * Mathf.Rad2Deg;

        Vector3 vCross = Vector3.Cross(selfTransform.forward, dir);
        if (degree > 15)
        {
            if (vCross.y < 0)
                selfTransform.Rotate(0, -15, 0);
            else
                selfTransform.Rotate(0, 15, 0);
        }
        else
        {
            if (vCross.y < 0)
                selfTransform.Rotate(0, -degree, 0);
            else
                selfTransform.Rotate(0, degree, 0);
        }
    }


    public override AiState SwitchState()
    {
        //切至Dead (血量歸0
        if (npcData.Hp < 0.0001f)
        {
            RemoveChasingNpc();
            animator.SetTrigger("Dead");
            return new GolemDeadState(target, animator, selfTransform, npcHelper);
        }
        //被無雙打
        if (goWeakState && gm.Shield <= 0)
        {
            RemoveChasingNpc();
            animator.SetBool("NotReach", false);
            goWeakState = false;
            animator.SetTrigger("FeverAttack");
            return new GolemWeakState(target, animator, selfTransform, nowArmor, npcHelper);
        }

        float distance = (selfTransform.position - target.position).magnitude;

        System.Random random = new System.Random();
        int rnd = random.Next(1, 150);//判斷要不要用技能
        //Debug.Log(rnd);
        if (distance <= 12f && rnd < 4)
        {
            RemoveChasingNpc();

            if(rnd < 3)
            {
                animator.SetBool("NotReach", false);
                return new GolemSkillState(target, animator, selfTransform, nowArmor, npcHelper);
            }
            else
            {
                animator.SetBool("NotReach", false);
                return new GolemAttackState(target, animator, selfTransform, nowArmor, npcHelper);
            }
        }

        //到玩家旁邊切回idle
        if (distance <= attackDistance)
        {
            RemoveChasingNpc();
            animator.SetBool("NotReach", false);
            return new GolemIdleState(target, animator, selfTransform, nowArmor, npcHelper);
        }
        else if (distance > attackDistance)
        {
            return this;
        }


        throw new NotImplementedException();
    }
}

public class GolemWeakState : GolemBaseState
{
    //Npc npcData;
    Transform target;
    float showWeaknessTime;
    AnimatorStateInfo currentAnimation;
    bool GoWeakState = false;
    GolemManager gm;
    public GolemWeakState(Transform t, Animator a, Transform self, float armor, NpcHelper npcHelper) : base(a, self, npcHelper, armor)
    {
        //npcData = selfTransform.GetComponent<Npc>();
        target = t;
        nowArmor = armor;
        showWeaknessTime = 0;
        gm = (GolemManager)npcHelper;
        gm.dizzy = true;
    }
    public override void SetAnimation()
    {
        //Debug.Log($"Armor{nowArmor}");
        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);
        showWeaknessTime += Time.deltaTime;
        animator.SetBool("ShowWeakness", true);
        //Debug.Log(showWeaknessTime);
        //animator.SetFloat("WeakTime", showWeaknessTime);

        if (getHit != null)
        {
            if(getHit.DamageState.damageState == DamageState.TimePause)
            {
                showWeaknessTime -= 5f;
            }

            if (gm.Shield <= 0)
            {
                npcData.Hp -= getHit.Damage;
            }
            animator.ResetTrigger("getHit");
            animator.SetTrigger("getHit");

            if (getHit.DamageState.damageState == DamageState.Fever)
            {
                GoWeakState = true;
                gm.Shield -= 5;
            }
            //if (currentAnimation.IsName("GetHit0"))
            //{
            //    animator.SetTrigger("getHit2");
            //}
            //else
            //{
            //    animator.SetTrigger("getHit");
            //}


            nowArmor -= 1;
            getHit = null;
        }
    }

    public override AiState SwitchState()
    {

        //切至Dead (血量歸0
        if (npcData.Hp < 0.0001f)
        {
            animator.SetBool("ShowWeakness", false);
            animator.SetTrigger("Dead");
            gm.dizzy = false;
            return new GolemDeadState(target, animator, selfTransform, npcHelper);
        }

        //切至roar (血量低於一半
        if (npcData.Hp <= npcHelper.MaxHp / 2 && Once.CanSetShield == true)
        {
            animator.SetBool("ShowWeakness", false);
            animator.SetTrigger("SetShield");
            Once.CanSetShield = false;
            gm.dizzy = false;

            animator.ResetTrigger("getHit");
            return new GolemRoarState(target, animator, selfTransform, nowArmor, npcHelper);
        }

        if(GoWeakState)//被無雙打
        {
            animator.SetTrigger("FeverAttack");
            return new GolemWeakState(target, animator, selfTransform, nowArmor, npcHelper);
        }

        //露出時間結束 切回idle
        if (showWeaknessTime > WeakTime)
        {
            animator.SetBool("ShowWeakness", false);
            gm.dizzy = false;
            animator.ResetTrigger("getHit");
            return new GolemIdleState(target, animator, selfTransform, nowArmor, npcHelper);
        }
        //Armor被擊破 切至ArmorBreak
        //if (armor < 0)
        if (nowArmor <= 0)
        {
            animator.SetBool("ShowWeakness", false);
            animator.SetTrigger("ArmorBreak");
            animator.ResetTrigger("getHit");
            return new GolemArmorBreakState(target, animator, selfTransform, npcHelper);
        }


        //
        else return this;
    }

}

public class GolemArmorBreakState : GolemBaseState
{
    float armorValue = 12;
    Transform target;
    float time;
    GolemManager gm;

    public GolemArmorBreakState(Transform t, Animator a, Transform self, NpcHelper nh) : base(a, self, nh, 0)
    {
        var mcs = ObjectManager.MainCharacter.GetComponent<MainCharacterState>();
        mcs.FinishingReleased = false;
        target = t;
        time = 0;
        gm = (GolemManager)npcHelper;
        //npcData = selfTransform.GetComponent<Npc>();
    }

    public override void SetAnimation()
    {
        Debug.Log("armor break");
        time += Time.deltaTime;
        if (getHit != null)
        {
            if(getHit.DamageState.damageState == DamageState.TimePause)
            {
                time -= 5f;
            }

            if(getHit.Hit == HitType.finishing)
            {
                time += 20f;
            }

            animator.ResetTrigger("getHit");
            animator.SetTrigger("getHit");
            GolemManager gm = (GolemManager)npcHelper;
            if (gm.Shield <= 0)
            {
                npcData.Hp -= getHit.Damage;
            }
            getHit = null;
        }

    }

    public override AiState SwitchState()
    {
        //切至Dead (血量歸0
        if (npcData.Hp < 0.0001f)
        {
            animator.SetTrigger("Dead");
            animator.ResetTrigger("getHit");
            return new GolemDeadState(target, animator, selfTransform, npcHelper);
        }
        //暈眩時間結束 切回idle
        //Armor補滿
        if (time > 5)
        {
            Debug.Log("Backkkk");
            animator.SetTrigger("ArmorRecover");
            gm.dizzy = false;
            animator.ResetTrigger("getHit");
            return new GolemIdleState(target, animator, selfTransform, armorValue, npcHelper);
        }
        if (time <= 5)
        {
            return this;
        }
        throw new NotImplementedException();
    }
}
public class GolemAttackState : GolemBaseState
{
    Transform target;
    AnimatorStateInfo currentAnimation;
    bool finish = false;
    private bool goWeakState;
    float inStateTime = 0f;

    public GolemAttackState(Transform t, Animator a, Transform self, float armor, NpcHelper npcHelper) : base(a, self, npcHelper, armor)
    {
        nowArmor = armor;
        target = t;
        System.Random random = new System.Random();
        int attackType = random.Next(1, 3);

        float distance = (target.position - selfTransform.position).magnitude;

        if (distance > 4.5f)
        {
            animator.SetTrigger("Attack03");
        }
        if(distance <= 4.5f)
        {
            if (attackType == 1 )
            {
                animator.SetTrigger("Attack01");
            }
            else if (attackType == 2)
            {
                animator.SetTrigger("Attack02");
            }
        }
        //npcData = selfTransform.GetComponent<Npc>();
    }

    public override void SetAnimation()
    {
        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);

        if (currentAnimation.IsName("Attack01"))
        {
            finish = true;
        }
        if (getHit != null)
        {
            GolemManager gm = (GolemManager)npcHelper;
            if (gm.Shield <= 0)
            {
                npcData.Hp -= getHit.Damage;
            }
            if (getHit.DamageState.damageState == DamageState.Fever)
            {
                goWeakState = true;
                gm.Shield -= 5;
            }
            getHit = null;
        }
    }

    public override AiState SwitchState()
    {
        //切至Dead (血量歸0
        if (npcData.Hp < 0.0001f)
        {
            animator.SetTrigger("Dead");
            return new GolemDeadState(target, animator, selfTransform, npcHelper);
        }
        ////被完美閃避 短暫露出Armor Armor被擊破 切至ArmorBreak
        //if (AttackFlaw)
        //{
        //    animator.SetTrigger("ArmorBreak");
        //    return new GolemArmorBreakState(target, animator, selfTransform, npcHelper);
        //}

        //被無雙打
        GolemManager gm = (GolemManager)npcHelper;
        if (goWeakState && gm.Shield <= 0)
        {
            goWeakState = false;
            animator.SetTrigger("FeverAttack");
            return new GolemWeakState(target, animator, selfTransform, nowArmor, npcHelper);
        }

        inStateTime += Time.deltaTime;
        
        //if (!animator.IsInTransition(0) && !currentAnimation.IsName("Attack02") && !currentAnimation.IsName("Attack02 0") && !currentAnimation.IsName("Attack01") && inStateTime > 1)
        if (!animator.IsInTransition(0) && !currentAnimation.IsTag("Attack") && inStateTime > 1)
        {
            if (finish && gm.Shield <= 0)
            {
                //Attack01結束後 切至weak 
                return new GolemWeakState(target, animator, selfTransform, nowArmor, npcHelper);
            }
            else
                //Attack02結束後 切回idle
                return new GolemIdleState(target, animator, selfTransform, nowArmor, npcHelper);
        }

        return this;
    }
}
public class GolemSkillState : GolemBaseState
{
    Transform target;
    AnimatorStateInfo currentAnimation;
    private bool goWeakState;
    float inStateTime = 0;

    bool canInterrupt = false;
    GolemManager gm;
    float moveSpeed = 0;
    //float canMoveFramesOne = 50f;//Skill1
    float canMoveFramesTwo = 27f;//Skill2
    public bool canMove = false;

    //float freezeTime = 0;
    public GolemSkillState(Transform t, Animator a, Transform self, float armor, NpcHelper nh) :  base(a, self, nh, armor)
    {
        gm = (GolemManager)npcHelper;
        //npcData = selfTransform.GetComponent<Npc>();
        target = t;
        nowArmor = armor;
        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);

        System.Random random = new System.Random();
        int attackType = random.Next(1, 3);
        Debug.Log(attackType);
        if (attackType == 1)
        {
            animator.SetTrigger("Skill");
            //UiManager.singleton.ShowSikaTip("ItemIceTips");
            Debug.Log("Showwwwwwwwwwwwwwwwwwwwwww");
        }
        else if (attackType == 2  && npcData.Hp <= npcHelper.MaxHp / 2)//低於一半血
        {
            animator.SetTrigger("Skill2");
            //UiManager.singleton.ShowSikaTip("ItemLockTips");
            Debug.Log("Showwwwwwwwwwwwwwwwwwwwwww");
        }
        else
        {
            attackType = random.Next(1, 3);
            animator.SetTrigger("Skill");
            //UiManager.singleton.ShowSikaTip("ItemIceTips");
            Debug.Log("Showwwwwwwwwwwwwwwwwwwwwww");
        }
    }

    public override void SetAnimation()
    {
        FreezeTime -= Time.deltaTime;

        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);

        if (currentAnimation.IsName("Skill 0")) moveSpeed = 0.3f;
        else if (currentAnimation.IsName("Skill2 0")) moveSpeed = 0.5f;

        
        if (!(currentAnimation.IsName("Skill") || currentAnimation.IsName("Skill 0")))// && !currentAnimation.IsName("Skill2 0")
            if(FreezeTime <= 5)
                LookAt();


        //if(currentAnimation.IsName("Skill") || currentAnimation.IsName("Skill 0"))//希卡之石使用提示
        //{
        //    //UiManager.singleton.ShowSikaTip("ItemIceTips");
        //    Debug.Log(" ");
        //}
        //else if (currentAnimation.IsName("Skill2") )
        //{
        //    //UiManager.singleton.ShowSikaTip("ItemLockTips");
        //    Debug.Log("  ");
        //}
        //else
        //{
        //    //UiManager.singleton.HideTip();
        //    //Debug.Log("hideeeeeeeeeeeeeeeeeeee");
        //}

        //freezeTime -= Time.deltaTime;
        if (currentAnimation.IsName("Skill 0"))//Skill1 程式位移
        {
            float dis = (target.position - selfTransform.position).magnitude;
            if (dis > 3f && FreezeTime <= 0)
            {
                if(dis>5f)//太近就不會追蹤
                    LookAt();

                selfTransform.Translate(0, 0, moveSpeed);
            }
        }

        if (currentAnimation.IsName("Skill2 0"))//Skill2 程式位移
        {
            if (canMoveFramesTwo > 0)//227 265
            {
                canMoveFramesTwo -= 1;
                Debug.Log("moveeeee");
                float dis = (target.position - selfTransform.position).magnitude;
                if (dis > 2f)
                {
                    selfTransform.Translate(0, 0, moveSpeed);
                }
            }
        }

        if(Once.IcePosision != Vector3.zero)
        {
            float iceToGolem = (Once.IcePosision - selfTransform.position).magnitude;
            if(iceToGolem <= 3 && (currentAnimation.IsName("Skill") || currentAnimation.IsName("Skill 0")))
            {
                Once.IceDestroyTime = 0f;
                AttackFlaw = true;
                if (gm.Shield > 0)//解護盾的方法
                {
                    gm.Shield -= 5;
                }
            }
        }

        if (getHit != null)
        {
            if(getHit.DamageState.damageState == DamageState.TimePause)
            {
                FreezeTime = 5f;
                //freezeTime = 5f;
            }
            
            if (getHit.DamageState.damageState == DamageState.TimePause && currentAnimation.IsName("Skill2 0 0 0"))
            {
                AttackFlaw = true;
                Debug.Log("innn");
            }

            if (getHit.DamageState.damageState == DamageState.Fever)
            {
                goWeakState = true;
                gm.Shield -= 5;
            }

            if (gm.Shield <= 0)
            {
                npcData.Hp -= getHit.Damage;
            }
            //else if(gm.Shield > 0 && getHit.DamageState.damageState == DamageState.Ice)
            //{
            //    gm.Shield -= getHit.Damage;
            //}

            getHit = null;
        }
    }
    public void LookAt()
    {
        Vector3 dir = target.position - selfTransform.position;
        dir.y = 0;
        dir.Normalize();
        float dot = Vector3.Dot(selfTransform.forward, dir);

        if (dot > 1) dot = 1;
        else if (dot < -1) dot = -1;

        float radian = Mathf.Acos(dot);
        float degree = radian * Mathf.Rad2Deg;

        Vector3 vCross = Vector3.Cross(selfTransform.forward, dir);
        if (degree > 15)
        {
            if (vCross.y < 0)
                selfTransform.Rotate(0, -15, 0);
            else
                selfTransform.Rotate(0, 15, 0);
        }
        else
        {
            if (vCross.y < 0)
                selfTransform.Rotate(0, -degree, 0);
            else
                selfTransform.Rotate(0, degree, 0);
        }
    }

    public override AiState SwitchState()
    {
        //切至Dead (血量歸0
        if (npcData.Hp < 0.0001f)
        {
            animator.SetTrigger("Dead");
            return new GolemDeadState(target, animator, selfTransform, npcHelper);
        }

        //被無雙打

        GolemManager gm = (GolemManager)npcHelper;
        if (goWeakState && gm.Shield <= 0)
        {
            goWeakState = false;
            animator.SetTrigger("FeverAttack");
            return new GolemWeakState(target, animator, selfTransform, nowArmor, npcHelper);
        }

        //玩家利用西卡之石破解技能 切至weak
        if (AttackFlaw && gm.Shield <= 0)
        {

            animator.SetTrigger("SheikahDefense");
            return new GolemWeakState(target, animator, selfTransform, nowArmor, npcHelper);
        }
        if (AttackFlaw)
        {
            animator.SetTrigger("HaveShieldGetHit");
            Debug.Log("Innn");

            return new GolemIdleState(target, animator, selfTransform, nowArmor, npcHelper);
        }
        //技能施放結束 切回idle
        inStateTime += Time.deltaTime;
        if (!animator.IsInTransition(0) && currentAnimation.IsName("Idle") && inStateTime > 1) //|| currentAnimation.IsName("Skill 0") || currentAnimation.IsName("Skill2") || currentAnimation.IsName("Skill2 0")
        {
            Debug.Log("hiiiiii");
            //Debug.Log(currentAnimation.IsName("Skill 0"));
            //Debug.Log(currentAnimation.IsName("Skill2"));
            //Debug.Log(currentAnimation.IsName("Skill2 0"));
            //Debug.Log("backkkkkkkkkkk");
            return new GolemIdleState(target, animator, selfTransform, nowArmor, npcHelper);
        }



        return this;
    }

    //public void Skill1Attack(float AttackSpeed)//事件觸發
    //{
    //    if (AttackSpeed == 0.4) canMove = true;//攻擊位移開關
    //    else canMove = false;
    //    Debug.Log("hiiiii");
    //    animator.SetFloat("Skill1AttackSpeed", AttackSpeed);
    //}
    //public void Skill2Attack(float AttackSpeed)//事件觸發
    //{

    //    if (AttackSpeed == 0.8) canMove = true;//攻擊位移開關
    //    else canMove = false;

    //    if (AttackSpeed == 0.2f)
    //    {
    //        canInterrupt = true;
    //    }
    //    else
    //    {
    //        canInterrupt = false;
    //    }
    //    animator.SetFloat("Skill2AttackSpeed", AttackSpeed);
    //    // +可用時間暫停中斷技能
    //    // getHit.DamageState.damageState == DamageState.TimePause 
    //}
}
public class GolemRoarState : GolemBaseState
{
    Transform target;
    float time = 0;
    AnimatorStateInfo currentAnimation;
    float inStateTime = 0f;
    public GolemRoarState(Transform t, Animator a, Transform self, float armor, NpcHelper nh) : base(a, self, nh, armor)
    {
        target = t;
        nowArmor = armor;

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
        //bool finish = false;
        //time += Time.deltaTime;
        //if (time > 4) finish = true;
        //Debug.Log(time);
        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);
        inStateTime += Time.deltaTime;
        if (!currentAnimation.IsName("Roar") && inStateTime > 1)
        {
            Debug.Log("back to idle");
            return new GolemIdleState(target, animator, selfTransform, nowArmor, npcHelper);
        }
        return this;
    }
}

public class GolemDeadState : GolemBaseState
{
    Transform target;
    public GolemDeadState(Transform t, Animator a, Transform self, NpcHelper nh) : base(a, self, nh, 0)
    {
        target = t;
        nh.Die();
    }

    public override void SetAnimation()
    {
    }

    public override AiState SwitchState()
    {
        return this;
    }
}



#endregion

#region Space State Machine
public abstract class SpaceBaseState : AiState
{
    Transform target;
    float armor;
    float max_armor;
    Npc npcData;
    public SpaceBaseState(Animator a, Transform self, NpcHelper nh) : base(a, self, nh, "", null)
    {
        animator = a;
        selfTransform = self;
        armor = max_armor;
        npcData = selfTransform.GetComponent<Npc>();
    }
}

public class SpaceIdleState : SpaceBaseState
{
    Transform target;
    public SpaceIdleState(Transform t, Animator a, Transform self, NpcHelper nh) : base(a, self, nh)
    {
        target = t;
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        //切至Chase

        //切至Attack

        //切至Skill

        //切至weak

        //切至Dead
        throw new NotImplementedException();
    }
}
public class SpaceChaseState : SpaceBaseState
{
    public SpaceChaseState(Animator a, Transform self, NpcHelper nh) : base(a, self, nh)
    {
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        //切至idle
        throw new NotImplementedException();
    }
}
public class SpaceAttackState : SpaceBaseState
{
    public SpaceAttackState(Animator a, Transform self, NpcHelper nh) : base(a, self, nh)
    {
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        //切至idle

        //切至Weak

        //切至Dead


        throw new NotImplementedException();
    }
}
public class SpaceSkillState : SpaceBaseState
{
    public SpaceSkillState(Animator a, Transform self, NpcHelper nh) : base(a, self, nh)
    {
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        //切至Weak

        //切至idle

        //切至Dead
        throw new NotImplementedException();
    }
}
public class SpaceWeakState : SpaceBaseState
{
    public SpaceWeakState(Animator a, Transform self, NpcHelper nh) : base(a, self, nh)
    {
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        //切至idle

        //切至ArmorBreak
        throw new NotImplementedException();
    }
}

public class SpaceArmorBreakState : SpaceBaseState
{
    public SpaceArmorBreakState(Animator a, Transform self, NpcHelper nh) : base(a, self, nh)
    {
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        //切至idle

        //切至Dead
        throw new NotImplementedException();
    }
}

public class SpaceDeadState : SpaceBaseState
{
    public SpaceDeadState(Animator a, Transform self, NpcHelper nh) : base(a, self, nh)
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




#endregion