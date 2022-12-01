using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class MainCharacterState : MonoBehaviour
{
    public float attackMoveDis;
    public bool bAttackMove = false;

    public GameObject focusLine;
    public GameObject Sword;
    public Animator animator;
    public AnimatorStateInfo currentAnimation;


    public float Fever;
    public int FeverTimes;


    IKController IK;
    float fTimer = 0f;
    bool dodge = false;
    bool frontMove = false;
    float time = 0f;
    public Transform newPlace;
    Npc npc;
    ThirdPersonCharacter tpc;
    // Start is called before the first frame update
    void Start()
    {
        npc = GetComponent<Npc>();
        tpc = GetComponent<ThirdPersonCharacter>();
        IK = GetComponent<IKController>();
    }

    // Update is called once per frame
    void Update()
    {
        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimation.IsName("Attack01") || currentAnimation.IsName("Attack01 0") || currentAnimation.IsName("Attack01 1") || currentAnimation.IsName("Attack01 2") || currentAnimation.IsName("GetHit") || currentAnimation.IsName("Die"))
            tpc.CanRotate = false;
        else tpc.CanRotate = true;

        //死亡
        if (npc.Hp <= 0)
        {
            animator.SetTrigger("died");
        }

        //被打
        if (Input.GetKeyDown(KeyCode.H)) //改成 l & h 區分輕擊 & 重擊
        {
            animator.SetTrigger("getHit");
            npc.Hp -= 20;//test
        }
        else  //???
        {
            //滯空時間？
            if (currentAnimation.IsName("BackFlip2") || currentAnimation.IsName("BackFlip"))
            {
                animator.SetTrigger("endHit");
            }
        }

        if (animator.IsInTransition(0) == false) //判斷是否在過度動畫
        {
            if (Input.GetMouseButtonDown(0) && (currentAnimation.IsName("Grounded") || currentAnimation.IsName("Attack01") || currentAnimation.IsName("Attack01 0") || currentAnimation.IsName("Attack01 1") || currentAnimation.IsName("Fast run")))
            {
                LeftMouseClick();//這命名不知道確切在幹嘛
            }
            if (Input.GetMouseButtonDown(1) && (currentAnimation.IsName("Attack01") || currentAnimation.IsName("Attack01 0") || currentAnimation.IsName("Attack01 1") || currentAnimation.IsName("Attack01 2")))
            {
                RightMouseClick();//這命名不知道確切在幹嘛
            }
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            //fTimer 也不知道這紀錄時間要用來幹嘛阿
            fTimer += Time.deltaTime;
            if (fTimer > 0.3)
            {
                Sword.SetActive(false);
                focusLine.SetActive(true);//???????
            }
            animator.SetFloat("dodge", fTimer);
        }
        //和 line 74. 一樣的判斷式，為什麼不寫在一起？
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            focusLine.SetActive(false);
            Sword.SetActive(true);
            fTimer = 0f;
            animator.SetFloat("dodge", fTimer);
        }

        //攻擊位移
        if (bAttackMove)// && (currentAnimation.IsName("Attack01"))   //何時 bAttackMove 會被改成 true?
        {
            transform.Translate(new Vector3(0, 0, 1) * 0.10f);
        }
        else
        {
            bAttackMove = false;
        }

        if (!(currentAnimation.IsName("Attack01") || currentAnimation.IsName("Attack01 0") || currentAnimation.IsName("Attack01 1") || currentAnimation.IsName("Attack01 2")))
        {
            animator.SetBool("attack02", false);
        }

        //增加 按下Lctrl閃避 時的位移距離

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
        if (time > 0f && time < 0.16f)
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
        Debug.DrawLine(transform.position, transform.position + transform.forward * 10f);

        if (currentAnimation.IsName("Fast run") || currentAnimation.IsName("Attack02 1") || currentAnimation.IsName("Attack02 2"))
        {
            IK.Weight_Up = 0;
        }
        else
        {
            IK.SetWeight_Up(1);
        }


        //for (int i = 0; i <= 70; i += 5)//攻擊範圍
        //{
        //    Debug.DrawRay(transform.position + (Vector3.up * 0.6f), Quaternion.Euler(0, i, 0) * transform.forward * 3.2f, Color.red);// 
        //    Debug.DrawRay(transform.position + (Vector3.up * 0.6f), Quaternion.Euler(0, -i, 0) * transform.forward * 3.2f, Color.red);
        //}
    }
    public virtual void LeftMouseClick()
    {
        animator.SetTrigger("attack01");
    }
    public virtual void RightMouseClick()
    {
        if (currentAnimation.IsName("Attack01") || currentAnimation.IsName("Attack01 0") || currentAnimation.IsName("Attack01 1") || currentAnimation.IsName("Attack01 2"))
            animator.SetBool("attack02", true);
        else
            animator.SetBool("attack02", false);
    }

    public void ForwardMove()　　//...建議如果是事件，加個綴字。
    {
        dodge = true;
    }

    public void AttackSpeedChange(float f)
    {
        animator.SetFloat("attackSpeed", f * 1.5f * 1.5f);
    }

    public void AnimationAttack(int attackType)
    {
        if (attackType == 1)
            NpcCommon.AttackDetection(transform.position, transform.forward, 140, 3.2f, true, new DamageData(10f, transform.forward * 0.10f, HitType.light, DamageStateInfo.NormalAttack));
        if (attackType == 2)
            NpcCommon.AttackDetection(transform.position, transform.forward, 140, 3.2f, true, new DamageData(10f, transform.forward * 0.15f, HitType.Heavy, DamageStateInfo.NormalAttack));
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
    public void AttackMoveOn(float dis) //...建議如果是事件，加個綴字。
    {
        if (!npc.collide)
        {
            bAttackMove = true;
            attackMoveDis = dis;
        }
    }
    public void AttackMoveOff() //...建議如果是事件，加個綴字。
    {
        bAttackMove = false;
    }
}

