using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.ThirdPerson;

public class MainCharacterState : MonoBehaviour, NpcHelper
{
    /// <summary>
    /// 速度線
    /// </summary>
    public GameObject focusLine;
    public GameObject Sword;
    public GameObject SwordEffect1;
    public GameObject SwordEffect2;
    public Animator animator;
    public AnimatorStateInfo currentAnimation;

    public float Fever;
    public int FeverTimes;

    IKController IK;
    float pressControlTime = 0f;
    bool dodge = false;
    bool frontMove = false;
    float time = 0f;
    public Transform newPlace;
    Npc npc;
    ThirdPersonCharacter tpc;



    bool FeverIk = false;
    private bool canRoll = true;

    public float Hp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool CanBeKockedOut => throw new NotImplementedException();

    public bool Dizzy => throw new NotImplementedException();

    public float MaxHp => throw new NotImplementedException();

    public float WeakPoint => throw new NotImplementedException();

    public float MaxWeakPoint => throw new NotImplementedException();

    public float Radius => 0.4f;

    public float CollisionDisplacement => 0.1f;

    public string Name => "莉可";

    public bool FinishingReleased { get; set; }
    bool isNightScene;
    void Awake()
    {

        var currentScene = SceneManager.GetActiveScene();
        var currentSceneName = currentScene.name;
        if (currentSceneName == "NightScene")
        {
            isNightScene = true;
        }
        else
        {
            isNightScene = false;
        }

        if (isNightScene)
        {
            ObjectManager.StateManagers.Add(this.gameObject.GetInstanceID(), this);
        }
        else
        {
            ObjectManager2.StateManagers.Add(this.gameObject.GetInstanceID(), this);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        if (isNightScene)
        {

            lst = ObjectManager.NpcsAlive.Values;
        }
        else
        {

            lst = ObjectManager2.NpcsAlive.Values;
        }

        npc = GetComponent<Npc>();
        tpc = GetComponent<ThirdPersonCharacter>();
        IK = GetComponent<IKController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (noHurt > 0) noHurt--;

        //處理美術位移
        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimation.IsName("Attack01") ||
            currentAnimation.IsName("Attack01 0") ||
            currentAnimation.IsName("Attack01 1") ||
            currentAnimation.IsName("Attack01 2") ||
            currentAnimation.IsName("Finishing") ||
            currentAnimation.IsName("Finishing") ||
            currentAnimation.IsName("GetHit") ||
            currentAnimation.IsName("Die") ||
            currentAnimation.IsName("Flying Back Death") ||
            currentAnimation.IsName("GettingUp03"))
            tpc.CanRotate = false;
        else tpc.CanRotate = true;

        //死亡
        if (PicoManager.Hp <= 0)
        {
            animator.SetTrigger("died");
        }


        if (Input.GetKeyDown(KeyCode.E) && !animator.IsInTransition(0) &&
            !(currentAnimation.IsName("Fever")
            || currentAnimation.IsName("Finishing")
            || currentAnimation.IsName("ThrowTimeStop")
            || currentAnimation.IsName("ThrowIce")
            || currentAnimation.IsName("ThrowBomb")
            || currentAnimation.IsName("Flying Back Death")
            || currentAnimation.IsName("GettingUp03")
            || currentAnimation.IsName("GetHit1")
            ))//斬殺技
        {
            CheckWeakEnemy();
        }

        //無雙條測試
        if (Input.GetKey(KeyCode.P))//增加無雙值
            AddPowerValue();

        if (Input.GetKeyDown(KeyCode.F) && !animator.IsInTransition(0) &&
            !(currentAnimation.IsName("Fever")
            || currentAnimation.IsName("Finishing")
            || currentAnimation.IsName("ThrowTimeStop")
            || currentAnimation.IsName("ThrowIce")
            || currentAnimation.IsName("ThrowBomb")
            || currentAnimation.IsName("Flying Back Death")
            || currentAnimation.IsName("GettingUp03")
            || currentAnimation.IsName("GetHit1"))
            )//使用無雙
        {
            if (PicoManager.Power >= 100)
            {
                PicoManager.Power -= PicoManager.PowerCost;
                animator.SetTrigger("Fever");
            }
        }


        if (animator.IsInTransition(0) == false) //判斷是否在過度動畫
        {
            if (Input.GetMouseButtonDown(0) && (currentAnimation.IsName("Grounded") || currentAnimation.IsName("Attack01") || currentAnimation.IsName("Attack01 0") || currentAnimation.IsName("Attack01 1") || currentAnimation.IsName("Fast run")))
            {
                LeftMouseClick();
            }
            if (Input.GetMouseButtonDown(1) && (currentAnimation.IsName("Attack01") || currentAnimation.IsName("Attack01 0") || currentAnimation.IsName("Attack01 1") || currentAnimation.IsName("Attack01 2")))
            {
                RightMouseClick();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (canRoll)
            {
                bool roll = true;
                pressControlTime += Time.deltaTime;
                if (pressControlTime > 0.3)//衝刺
                {
                    roll = false;
                    SwordEffect1.SetActive(true);
                    Sword.SetActive(false);
                    focusLine.SetActive(true);//速度線
                }
                if (currentAnimation.IsName("Fast run"))
                    SwordEffect1.SetActive(false);
                if (roll && !(currentAnimation.IsName("Fever") || currentAnimation.IsName("Finishing")))
                {
                    animator.Play("Front Dodge");
                }
                animator.SetFloat("dodge", pressControlTime);
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {

            focusLine.SetActive(false);
            SwordEffect2.SetActive(true);
            Sword.SetActive(true);
            pressControlTime = 0f;
            animator.SetTrigger("DodgeToIdle");
            animator.SetFloat("dodge", pressControlTime);
        }
        DodgeTranslate();

        //攻擊位移
        if (attackMove > 0 && !npc.collideFront)// && (currentAnimation.IsName("Attack01"))   //何時 bAttackMove 會被改成 true?
        {
            attackMove--;
            transform.Translate(new Vector3(0, 0, 1) * 0.10f);
        }

        //重攻擊處理
        if (!(currentAnimation.IsName("Attack01") || currentAnimation.IsName("Attack01 0") || currentAnimation.IsName("Attack01 1") || currentAnimation.IsName("Attack01 2")))
        {
            animator.SetBool("attack02", false);
        }

        //IK調整
        if ((currentAnimation.IsName("Fast run") || currentAnimation.IsName("Attack02 1") || currentAnimation.IsName("Attack02 2") || currentAnimation.IsName("Finishing")) || FeverIk)
        {
            IK.Weight_Up = 0;
        }
        else
        {
            IK.SetWeight_Up(1);
        }
    }

    private void AddPowerValue()
    {
        PicoManager.Power++;
    }

    /// <summary>
    /// 增加 按下Lctrl閃避 時的位移距離
    /// </summary>
    private void DodgeTranslate()
    {
        //何時 dodge 會被改成 true?
        if (dodge)//(Input.GetKeyDown(KeyCode.LeftControl) && (currentAnimation.IsName("Grounded") || currentAnimation.IsName("Front Dodge")))
        {
            //print("dodge----------");
            frontMove = true;
        }
        if (frontMove)
        {
            //上面才有個 fTimer， 這裡又一個 time ，差在哪？
            time += Time.deltaTime;
        }
        float dodgeEndingTime = 0.16f;
        if (time > 0f && time < dodgeEndingTime)
        {
            if (!npc.collide)
            {
                tpc.artistMovement = true;
                transform.Translate(new Vector3(0f, 0f, 1f) * 0.15f);
            }
            else
            {
                tpc.artistMovement = false;
            }
        }
        else if (time >= 0.17f)
        {
            frontMove = false;
            dodge = false;
            time = 0f;
        }
    }

    public virtual void LeftMouseClick()
    {
        //輕攻擊派生技： 由同一個 trigger 串接 不同的輕攻擊動作。
        animator.SetTrigger("attack01");
    }
    public virtual void RightMouseClick()
    {
        //Simon: 註解自己打
        if (currentAnimation.IsName("Attack01") || currentAnimation.IsName("Attack01 0") || currentAnimation.IsName("Attack01 1") || currentAnimation.IsName("Attack01 2"))
            animator.SetBool("attack02", true); //應該要用trigger...不過  來不及ㄌXD
        else
            animator.SetBool("attack02", false);
    }

    public void CheckWeakEnemy()
    {
        foreach (var i in lst)
        {
            if ((i.transform.position - transform.position).magnitude < 4f)
            {
                Dictionary<int, NpcHelper> managers;
                if (isNightScene)
                {
                    managers = ObjectManager.StateManagers;
                }
                else
                {
                    managers = ObjectManager2.StateManagers;
                }
                var nh = managers[i.GetInstanceID()];
                if (nh.WeakPoint <=0)
                {
                    animator.SetTrigger("Finishing");
                    FinishingReleased = true;
                    break;
                }
            }
        }
    }

    public void ForwardMove()　　//...建議如果是事件，加個綴字。
    {
        dodge = true;
    }

    public void FeverIkControl()
    {
        FeverIk = false;
    }
    public void FeverAttackSpeed(float speed)//事件觸發
    {
        print("123");
        if (speed > 1)
        {
            FeverIk = true;
        }
        else if (speed < 1)
        {
            FeverIk = false;
        }
        animator.SetFloat("FeverAttackSpeed", speed);
    }

    public void FinshAttackSpeed(float speed)//事件觸發
    {
        animator.SetFloat("FinshAttackSpeed", speed);
    }

    public void AttackSpeedChange(float f)//事件觸發
    {
        animator.SetFloat("attackSpeed", f * 1.5f * 1.5f);
    }

    public void AnimationAttack(int attackType)//事件觸發
    {
        if (attackType == 1)//普攻34
            NpcCommon.AttackDetection("Pico", transform.position, transform.forward, 140, 3.2f, true, new DamageData(10f, transform.forward * 1f, HitType.light, DamageStateInfo.NormalAttack), "Npc");
        if (attackType == 2)//重擊1
            NpcCommon.AttackDetection("Pico", transform.position + transform.forward * 0.7f, transform.forward, 360, 2.5f, true, new DamageData(20f, transform.forward * 0.15f, HitType.Heavy, DamageStateInfo.NormalAttack), "Npc");
        if (attackType == 3)//重擊2
            NpcCommon.AttackDetection("Pico", transform.position + transform.forward * -0.2f, transform.forward, 180, 4.5f, true, new DamageData(20f, transform.forward * 0.15f, HitType.Heavy, DamageStateInfo.NormalAttack), "Npc");
        if (attackType == 4)//終結技
            NpcCommon.AttackDetection("Pico", transform.position, transform.forward, 180, 4f, true, new DamageData(100f, transform.forward * 0.15f, HitType.finishing, DamageStateInfo.NormalAttack), "Npc");
        if (attackType == 5)//無雙
            NpcCommon.AttackDetection("Pico", transform.position, transform.forward, 360, 13f, true, new DamageData(200f, transform.forward * 0.15f, HitType.Heavy, new DamageStateInfo(DamageState.Fever, 0)), "Npc");



    }

    #region  OnDrawGizmos 註解
    //public LayerMask LY;
    //private void OnDrawGizmos()
    //{
    //    for (int i = 0; i <= 55; i += 5)
    //    {
    //        if ( Physics.Raycast(transform.position + (Vector3.up * 0.6f), Quaternion.Euler(0, i, 0) * transform.forward * 3.2f,out var info,3.2f, LY)&& info.transform.tag =="Npc")
    //        {
    //            Gizmos.color = Color.green;
    //            Gizmos.DrawLine(transform.position + (Vector3.up * 0.6f), transform.position + (Vector3.up * 0.6f) + Quaternion.Euler(0, i, 0) * transform.forward * 3.2f);
    //        }
    //        else
    //        {
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawLine(transform.position + (Vector3.up * 0.6f), transform.position + (Vector3.up * 0.6f) + Quaternion.Euler(0, i, 0) * transform.forward * 3.2f);
    //        }

    //        if (Physics.Raycast(transform.position + (Vector3.up * 0.6f), Quaternion.Euler(0, -i, 0) * transform.forward * 3.2f, out info,3.2f, LY) && info.transform.tag != "Npc")
    //        {
    //            Gizmos.color = Color.green;
    //            Gizmos.DrawLine(transform.position + (Vector3.up * 0.6f), transform.position + (Vector3.up * 0.6f) + Quaternion.Euler(0, -i, 0) * transform.forward * 3.2f);
    //        }
    //        else
    //        {
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawLine(transform.position + (Vector3.up * 0.6f), transform.position + (Vector3.up * 0.6f) + Quaternion.Euler(0, -i, 0) * transform.forward * 3.2f);
    //        }
    //    }
    //}

    #endregion
    int attackMove;
    public void AttackMoveOn(int keepFrame) //...建議如果是事件，加個綴字。
    {
        if (!npc.collide)
        {
            attackMove = keepFrame;
        }
    }

    public void GetHurt(DamageData damageData)
    {
        Debug.Log(noHurt);
        if (noHurt > 0) return;

        Debug.Log("hurt");

        PicoManager.Hp -= damageData.Damage;

        //被打
        if (damageData.Hit == HitType.light) //改成 l & h 區分輕擊 & 重擊
        {
            animator.SetTrigger("getHit");
        }
        else if (damageData.Hit == HitType.Heavy)
        {
            animator.SetTrigger("getHeavyHit");
            npc.KnockOff(damageData.Force);
        }
    }
    int noHurt;
    private Dictionary<int, GameObject>.ValueCollection lst;

    public void SetNoHurt(int keepFrame)
    {
        noHurt = keepFrame;
    }

    public void Move()
    {
        throw new NotImplementedException();
    }

    public void Turn(Vector3 direction)
    {
        throw new NotImplementedException();
    }

    public void Look(Transform target)
    {
        throw new NotImplementedException();
    }

    public void KnockedOut_RollDisabled()
    {
        canRoll = false;
    }
    public void KnockedOut_RollEnabled()
    {
        canRoll = true;
    }
    void FootL()
    {
    }
    void FootR()
    {
    }

    public void Die()
    {
        throw new NotImplementedException();
    }
}

