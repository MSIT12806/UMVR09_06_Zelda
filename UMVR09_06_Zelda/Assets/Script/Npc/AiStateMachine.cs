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
    IKController ik;
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
        ik = selfTransform.GetComponent<IKController>();
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

        AiStateCommon.LookAtByIk(ik, ObjectManager.MainCharacterHead);
        AiStateCommon.Turn(selfTransform, target.position - selfTransform.position);
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
        attackRange = r > 0.3 ? 10f : 2f;
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
        self.GetComponent<IKController>().LookAtObj = null;
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
        //if (npc.Hp <= 0)
        //{
        //    deadTime += Time.deltaTime;
        //    if (getHit != null)
        //    {
        //        deadTime = 0f;
        //        animator.SetTrigger("toFlog");
        //        Debug.Log("Stop FLOGINGGGGG!!!!!!");
        //    }
        //}
        //else
        //{
        //    deadTime = 0f;
        //}
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

            npc.nextPosition = selfTransform.position + getHit.Force;
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
            var particleSystem = selfTransform.FindAnyChild<Transform>("FX_NPC_Die");
            var fxGo = particleSystem.gameObject;
            fxGo.transform.parent = null;
            fxGo.SetActive(true);
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
    public DragonIdleState(Transform t, Animator a, Transform self, NpcHelper nh) : base(a, self, nh, "Idle", t.GetComponent<PicoState>())
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

        if ((int)stage.gameState == 2)
            return new DragonFlyState(target, animator, selfTransform, npcHelper);


        return this;
    }
}

public class DragonFightState : AiState
{
    Transform target;
    Transform head;
    float flyHpLimit;
    float attackWait = AiStateCommon.RandonAttackScale();
    float dazeSeconds;
    public DragonFightState(Transform t, Animator a, Transform self, NpcHelper nh) : base(a, self, nh, "Fight", t.GetComponent<PicoState>())
    {
        target = t;
        head = self.FindAnyChild<Transform>("Head");
        flyHpLimit = npcHelper.Hp / 2;
        RefreshDazeTime();
    }

    public void RefreshDazeTime()
    {
        animator.SetBool("Move", false);
        dazeSeconds = UnityEngine.Random.Range(1, 10);
    }
    public override void SetAnimation()
    {
        AiStateCommon.Turn(selfTransform, target.position - selfTransform.position);
        AiStateCommon.Look(head, target);
    }

    public override AiState SwitchState()
    {
        if (npcHelper.Hp <= 0) return new DragonDeathState(animator, selfTransform, npcHelper);
        if (npcHelper.Hp < flyHpLimit)
        {
            return new DragonFlyState(target, animator, selfTransform, npcHelper);
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
            return new DragonAttackState(target, "TailHit", animator, selfTransform, npcHelper);
        }
        if (distance <= 8f)
        {
            return new DragonAttackState(target, "FireHit", animator, selfTransform, npcHelper);
        }
        else
        {
            return new DragonChaseState(target, animator, selfTransform, npcHelper);
        }

    }
}

/// <summary>
/// 可以說是 飛翔狀態的 FightState
/// </summary>
public class DragonFlyState : AiState
{
    Transform target;
    float dazeSeconds;
    Transform head;
    Vector3 turnPoint;
    public DragonFlyState(Transform target, Animator a, Transform self, NpcHelper nh) : base(a, self, nh, "Fight", target.GetComponent<PicoState>())
    {
        head = self.FindAnyChild<Transform>("Head");
        this.target = target;
        //不再受到任何攻擊，除非將其擊落(丟炸彈)
        animator.SetBool("Fly", nh.Hp >= nh.MaxHp);
        //一直噴火球兒
    }

    public void RefreshDazeTime()
    {
        animator.SetBool("Move", false);
        dazeSeconds = UnityEngine.Random.Range(1, 10);
    }
    public override void SetAnimation()
    {
        AiStateCommon.Turn(selfTransform, target.position - selfTransform.position);
        //AiStateCommon.Look(head, target);
    }

    public override AiState SwitchState()
    {
        if (npcHelper.Hp <= 0) return new DragonDeathState(animator, selfTransform, npcHelper);
        dazeSeconds -= Time.deltaTime;
        if (dazeSeconds > 0) return this;

        var distance = Vector3.Distance(selfTransform.position, target.position);
        // 距離 >10 || <5 追
        if (distance > 10 || distance < 5)
        {
            var randomDir = new Vector3(UnityEngine.Random.value, 0, UnityEngine.Random.value).normalized;
            var flyPoint = target.position + randomDir * 7.5f;
            return new DragonFlyChaseState(this, animator, selfTransform, npcHelper, flyPoint);
        }
        else
        {
            // 發呆完吐火球
            if (dazeSeconds <= 0)
            {
                animator.SetTrigger("FireHit");
            }
            RefreshDazeTime();
            return this;
        }
    }
}
public class DragonFlyChaseState : AiState
{
    /*
* 1. 保持 5~10公尺的距離， 太遠或太近就使用這個狀態來移動。
*/

    Vector3 vec;
    int flyFrameCount = 150;
    DragonFlyState flyFightState;
    public DragonFlyChaseState(DragonFlyState flyState, Animator a, Transform self, NpcHelper nh, Vector3 position) : base(a, self, nh, "Chase", null)
    {
        flyFightState = flyState;
        vec = (position - self.position).normalized;
    }
    bool move;
    public override void SetAnimation()
    {
        if (!move)
        {
            move = !AiStateCommon.Turn(selfTransform, vec);
        }
        if (move)
            animator.SetBool("Move", true);

    }

    public override AiState SwitchState()
    {
        if (move)
            flyFrameCount--;

        if (flyFrameCount < 1)
        {
            flyFightState.RefreshDazeTime();
            return flyFightState;
        }

        Debug.Log(flyFrameCount);
        return this;
    }
}
public class DragonChaseState : AiState
{
    Transform target;
    public DragonChaseState(Transform t, Animator a, Transform self, NpcHelper nh) : base(a, self, nh, "Chase", null)
    {
        target = t;
    }

    public override void SetAnimation()
    {
        //面向 pico
        //朝著 pico 用 aStar 做 seek ，直到攻擊範圍內
    }

    public override AiState SwitchState()
    {
        if (npcHelper.Hp <= 0) return new DragonDeathState(animator, selfTransform, npcHelper);

        return this;
    }
}
public class DragonAttackState : AiState
{
    bool attack;
    string triggerName;
    Transform target;
    DragonFightState fightState;
    public DragonAttackState(DragonFightState state,Transform transform, string trigger, Animator a, Transform self, NpcHelper nh) : base(a, self, nh, "Attack", null)
    {
        this.triggerName = trigger;
        this.target = transform;
        fightState = state;
    }
    public override void SetAnimation()
    {
        animator.SetTrigger(triggerName);
        attack = true;
        //if (getHit != null)
        //{
        //    if (npcHelper.Hp <= 0)
        //    {
        //        animator.Play("Die");
        //    }
        //}

    }
    public override AiState SwitchState()
    {
        if (npcHelper.Hp <= 0) return new DragonDeathState(animator, selfTransform, npcHelper);
        if (attack)
        {
            var r = UnityEngine.Random.value;
            if(r < 0.3f)
            {
                animator.SetTrigger("BasicHit");
            }
            else if (r < 0.6f)
            {
                animator.SetTrigger("TailHit");
            }
            else if (r < 0.6f)
            {
                animator.SetTrigger("FireHit");
            }
            fightState.RefreshDazeTime();
            return fightState;
        }


        return this;
    }
}
public class DragonDizzyState : AiState
{
    public DragonDizzyState(Animator a, Transform self, NpcHelper nh) : base(a, self, nh, "Dizzy", null)
    {
    }

    public override void SetAnimation()
    {
        throw new NotImplementedException();
    }

    public override AiState SwitchState()
    {
        if (npcHelper.Hp <= 0) return new DragonDeathState(animator, selfTransform, npcHelper);

        return this;
    }
}
public class DragonDeathState : AiState
{
    public DragonDeathState(Animator a, Transform self, NpcHelper nh) : base(a, self, nh, "Death", null)
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
public static class AiStateCommon
{

    public static bool Turn(Transform body, Vector3 direction)
    {
        var degree = Vector3.SignedAngle(body.forward.WithY(), direction.WithY(), Vector3.up);
        if (degree < -1)
        {
            body.Rotate(Vector3.up, -1);
            return true;

        }
        else if (degree > 1)
        {
            body.Rotate(Vector3.up, 1);
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
#endregion

#region Golem State Machine
public static class Once
{
    public static bool CanSetShield = true;
}
public abstract class GolemBaseState : AiState
{

    protected Npc npcData;
    protected float max_armor = 10;
    protected float armor;
    public float WeakTime = 5;//弱點持續時間
    public float ArmorBreakTime = 5; //破甲暈眩持續時間 
    public bool AttackFlaw = false;
    public DamageData GolemDamageData;
    public GolemBaseState(Animator a, Transform self, float armor, NpcHelper nh) : base(a, self, nh, "", null)//菁英怪 & Boss 有盾值
    {
        animator = a;
        selfTransform = self;
        //armor = max_armor;
        npcData = selfTransform.GetComponent<Npc>();
    }
}
public class GolemIdleState : GolemBaseState
{
    float attackDistance = 3.3f;
    float nowArmor;
    Transform target;
    PicoState picoState;
    bool goWeakState = false;
    public GolemIdleState(Transform t, Animator a, Transform self, float armor, NpcHelper nh) : base(a, self, armor, nh)
    {
        target = t;
        nowArmor = armor;
        picoState = target.GetComponent<PicoState>();
        //npcData = selfTransform.GetComponent<Npc>();
    }
    public override void SetAnimation()
    {
        //Debug.Log("111111");
        if (getHit != null)
        {
            Debug.Log($"before {npcData.Hp}");
            npcData.Hp -= getHit.Damage / 10;
            if(getHit.Hit == HitType.fever)
            {
                goWeakState = true;
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
        if (npcData.Hp <= 10 && Once.CanSetShield)
        {
            Once.CanSetShield = false;
            animator.SetTrigger("SetShield");
            return new GolemRoarState(target, animator, selfTransform, npcHelper);
        }

        //被無雙技打
        if (goWeakState)
        {
            goWeakState = false;
            animator.SetTrigger("FeverAttack");
            return new GolemWeakState(target, animator, selfTransform, nowArmor, npcHelper);
        }
        
        //切至Attack (追到後就打? 或亂數決定
        float distance = (target.position - selfTransform.position).magnitude;
        if (distance <= attackDistance)
        {
            //animator.SetTrigger("Attack01");
            return new GolemAttackState(target, animator, selfTransform, nowArmor, npcHelper);
        }

        ////切至Weak (攻擊後就露出? 或亂數決定
        //bool weak = false;
        //if (weak)
        //{
        //    animator.SetBool("ShowWeakness", true);
        //    return new GolemWeakState(target, animator, selfTransform, nowArmor, WeakTime, npcHelper);
        //}

        //切至Skill (血量到特定%? 或亂數決定
        bool skill = false;
        if (skill)
        {
            animator.SetTrigger("Skill");
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
    float nowArmor;
    float attackDistance = 3.3f;
    AnimatorStateInfo currentAnimation;
    private bool goWeakState;

    public GolemChaseState(Transform t, Animator a, Transform self, NpcHelper nh, float armor) : base(a, self, armor, nh)
    {
        target = t;
        nowArmor = armor;
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
        LookAt();
        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);
        if (!animator.IsInTransition(0) && currentAnimation.IsName("Walk"))
            selfTransform.Translate(0, 0, 0.8f);

        if (getHit != null)
        {
            npcData.Hp -= getHit.Damage / 10;
            if (getHit.Hit == HitType.fever)
            {
                goWeakState = true;
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
            animator.SetTrigger("Dead");
            return new GolemDeadState(target, animator, selfTransform, npcHelper);
        }
        //被無雙打
        if (goWeakState)
        {
            goWeakState = false;
            animator.SetTrigger("FeverAttack");
            return new GolemWeakState(target, animator, selfTransform, armor, npcHelper);
        }

        //到玩家旁邊切回idle
        float distance = (selfTransform.position - target.position).magnitude;
        if (distance <= attackDistance)
        {
            RemoveChasingNpc();
            animator.SetBool("NotReach", false);
            return new GolemIdleState(target, animator, selfTransform, armor, npcHelper);
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
    public GolemWeakState(Transform t, Animator a, Transform self, float armor, NpcHelper npcHelper) : base(a, self, armor, npcHelper)
    {
        //npcData = selfTransform.GetComponent<Npc>();
        target = t;
        showWeaknessTime = 0;
    }
    public override void SetAnimation()
    {
        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);
        showWeaknessTime += Time.deltaTime;
        animator.SetBool("ShowWeakness", true);
        //Debug.Log(showWeaknessTime);
        //animator.SetFloat("WeakTime", showWeaknessTime);


        if (getHit != null)
        {
            npcData.Hp -= getHit.Damage / 10;
            if (currentAnimation.IsName("GetHit0"))
                animator.SetTrigger("getHit2");
            else
                animator.SetTrigger("getHit");
            armor -= 1;
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
            return new GolemDeadState(target, animator, selfTransform, npcHelper);
        }
        //露出時間結束 切回idle
        if (showWeaknessTime > WeakTime)
        {
            animator.SetBool("ShowWeakness", false);
            return new GolemIdleState(target, animator, selfTransform, armor, npcHelper);
        }
        //Armor被擊破 切至ArmorBreak
        //if (armor < 0)
        if (false)
        {
            //animator.SetBool("ShowWeakness", false);
            animator.SetTrigger("ArmorBreak");
            return new GolemArmorBreakState(target, animator, selfTransform, npcHelper);
        }


        //
        else return this;
    }

}

public class GolemArmorBreakState : GolemBaseState
{
    float armorValue = 6;
    Transform target;
    float time;

    public GolemArmorBreakState(Transform t, Animator a, Transform self, NpcHelper nh) : base(a, self, 0, nh)
    {
        target = t;
        time = 0;
        //npcData = selfTransform.GetComponent<Npc>();
    }

    public override void SetAnimation()
    {
        time += Time.deltaTime;
        if (getHit != null)
        {
            animator.SetTrigger("getHit");
            npcData.Hp -= getHit.Damage / 10 + 3;
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
        //暈眩時間結束 切回idle
        //Armor補滿
        if (time > 5)
        {
            animator.SetTrigger("ArmorRecover");
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
    float nowArmor;
    private bool goWeakState;

    public GolemAttackState(Transform t, Animator a, Transform self, float armor, NpcHelper npcHelper) : base(a, self, armor, npcHelper)
    {
        nowArmor = armor;
        target = t;
        System.Random random = new System.Random();
        int attackType = random.Next(1, 2);
        if (attackType == 1)
        {
            animator.SetTrigger("Attack01");
        }
        else if (attackType == 2)
        {
            animator.SetTrigger("Attack02");
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
            npcData.Hp -= getHit.Damage / 10;
            if (getHit.Hit == HitType.fever)
            {
                goWeakState = true;
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
        if (goWeakState)
        {
            goWeakState = false;
            animator.SetTrigger("FeverAttack");
            return new GolemWeakState(target, animator, selfTransform, armor, npcHelper);
        }

        if (!currentAnimation.IsName("Attack02") && !currentAnimation.IsName("Attack01"))
        {
            if (finish)
            {
                //Attack01結束後 切至weak 
                return new GolemWeakState(target, animator, selfTransform, nowArmor, npcHelper);
            }
            else
                //Attack02結束後 切回idle
                return new GolemIdleState(target, animator, selfTransform, armor, npcHelper);
        }

        return this;
    }
}
public class GolemSkillState : GolemBaseState
{
    Transform target;
    AnimatorStateInfo currentAnimation;
    float nowArmor;
    private bool goWeakState;

    public GolemSkillState(Transform t, Animator a, Transform self, float armor ,NpcHelper nh) : base(a, self, 0, nh)
    {
        //npcData = selfTransform.GetComponent<Npc>();
        target = t;
        nowArmor = armor;
    }

    public override void SetAnimation()
    {
        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);

        if (getHit != null)
        {
            //待修改
            if (getHit.Hit == HitType.Ice)
            {
                AttackFlaw = true;
            }
            if(getHit.Hit == HitType.fever)
            {
                goWeakState = true;
            }

            npcData.Hp -= getHit.Damage / 10;
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

        //被無雙打
        if (goWeakState)
        {
            goWeakState = false;
            animator.SetTrigger("FeverAttack");
            return new GolemWeakState(target, animator, selfTransform, armor, npcHelper);
        }

        //玩家利用西卡之石破解技能 切至ArmorBreak
        if (AttackFlaw)
        {
            animator.SetTrigger("SheikahDefense");
            return new GolemWeakState(target, animator, selfTransform,nowArmor, npcHelper);
        }
        //技能施放結束 切回idle
        if (!currentAnimation.IsName("Skill"))
        {
            return new GolemIdleState(target, animator, selfTransform, armor, npcHelper);
        }



        return this;
    }
}
public class GolemRoarState : GolemBaseState
{
    Transform target;
    float time = 0;
    public GolemRoarState(Transform t, Animator a, Transform self, NpcHelper nh) : base(a, self, 0, nh)
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
        time += Time.deltaTime;
        if (time > 4) finish = true;
        Debug.Log(time);
        if (finish)
        {
            Debug.Log("back to idle");

            return new GolemIdleState(target, animator, selfTransform, armor, npcHelper);
        }
        return this;
    }
}

public class GolemDeadState : GolemBaseState
{
    Transform target;
    public GolemDeadState(Transform t, Animator a, Transform self, NpcHelper nh) : base(a, self, 0, nh)
    {
        target = t;
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