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
    private float fTimer = 0f;
    bool dodge = false;
    Vector3 newPos;
    bool frontMove = false;
    float time = 0f;
    public Transform newPlace;
    Npc n;
    ThirdPersonCharacter tpc;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 newPos = transform.position;
        n = GetComponent<Npc>();
        tpc = GetComponent<ThirdPersonCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        currentAnimation = animator.GetCurrentAnimatorStateInfo(0);
        var tpc = this.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>();
        if (currentAnimation.IsName("Attack01") || currentAnimation.IsName("Attack01 0") || currentAnimation.IsName("Attack01 1") || currentAnimation.IsName("Attack01 2") || currentAnimation.IsName("GetHit") || currentAnimation.IsName("Die"))
            tpc.CanRotate = false;
        else tpc.CanRotate = true;
        //if(asi.IsName(nowAsiName))
        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("died");
            //animator.SetFloat("hp", 0.0f);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            animator.SetTrigger("getHit");
            n.Hp -= 20;//test
        }
        else
        {
            //滯空時間？
            if (currentAnimation.IsName("BackFlip2") || currentAnimation.IsName("BackFlip"))
            {
                animator.SetTrigger("endHit");
            }
        }

        if (animator.IsInTransition(0) == false)
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

        if (Input.GetKey(KeyCode.LeftControl))
        {
            fTimer += Time.deltaTime;
            if (fTimer > 0.3)
            {
                Sword.SetActive(false);
                focusLine.SetActive(true);
            }
            animator.SetFloat("dodge", fTimer);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            focusLine.SetActive(false);
            Sword.SetActive(true);
            fTimer = 0f;
            animator.SetFloat("dodge", fTimer);
        }
        //else
        //{
        //    fTimer = 0f;
        //    animator.SetFloat("dodge", fTimer);
        //}

        //沒用 大概
        //var a = this.GetComponent<IKController>();
        //if (currentAnimation.IsName("Attack02 1"))
        //{
        //    a.IkActive = false;
        //    //Sword.SetActive(false);
        //}
        //if (currentAnimation.IsName("Fast run"))
        //{
        //    a.IkActive = false;
        //    //Sword.SetActive(false);
        //}
        //if (currentAnimation.IsName("Grounded"))
        //{
        //    a.IkActive = true;
        //    //Sword.SetActive(true);
        //}

        //攻擊位移
        if (bAttackMove)// && (currentAnimation.IsName("Attack01"))
        {
            transform.Translate(new Vector3(0, 0, 1) * 0.15f);
        }
        else
        {
            bAttackMove = false;
        }

        if (currentAnimation.IsName("Attack01"))
        {
            //Vector3 ForwardMove = transform.position;
            //ForwardMove.z += 3f*Time.deltaTime;
            //transform.position = transform.position + transform.forward * 0.2f;
        }

        if (!(currentAnimation.IsName("Attack01") || currentAnimation.IsName("Attack01 0") || currentAnimation.IsName("Attack01 1") || currentAnimation.IsName("Attack01 2")))
        {
            animator.SetBool("attack02", false);
        }

        //增加 按下Lctrl閃避 時的位移距離

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    newPos = transform.position += transform.forward * 5f;
        //}
        //Vector3.Lerp(transform.position, newPos, 0.5f);

        if (dodge)//(Input.GetKeyDown(KeyCode.LeftControl) && (currentAnimation.IsName("Grounded") || currentAnimation.IsName("Front Dodge")))
        {
            //print("dodge----------");
            frontMove = true;
        }
        if (frontMove)
        {
            time += Time.deltaTime;
        }
        if (time > 0f && time < 0.16f)
        {
            if (!n.collide)
            {
                //print("????????");
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

        var IK = GetComponent<IKController>();
        if (currentAnimation.IsName("Fast run") || currentAnimation.IsName("Attack02 1") || currentAnimation.IsName("Attack02 2"))
            IK.Weight_Up = 0;
        else
            IK.SetWeight_Up(1);


        //if (Input.GetMouseButtonDown(0))
        //{
        //    //transform.position = transform.position + transform.forward * 10f;
        //    transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward.normalized * 2f, 0.1f);
        //}


        for (int i = 0; i <= 70; i += 5)//攻擊範圍
        {
            Debug.DrawRay(transform.position + (Vector3.up * 0.6f), Quaternion.Euler(0, i, 0) * transform.forward * 3.2f, Color.red);// 
            Debug.DrawRay(transform.position + (Vector3.up * 0.6f), Quaternion.Euler(0, -i, 0) * transform.forward * 3.2f, Color.red);
        }
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

    public void ForwardMove()
    {
        dodge = true;
        //設定true false開關外面主程式來位移------------------------------------------------------------------------------
        //transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward.normalized * 1f, 1f);
        //print(transform.position);
        //transform.position = transform.position + transform.forward * 100f;
        //print(transform.position);

    }

    public void AttackSpeedChange(float f)
    {
        animator.SetFloat("attackSpeed", f * 1.5f * 1.5f);
        //print(231321213);
    }

    public void AnimationAttack(int attackType)
    {
        if (attackType == 1)
            NpcCommon.AttackDetection(transform.position, transform.forward, 140, 3.2f, true, new DamageData(10f, transform.forward * 0.10f, HitType.light, DamageStateInfo.NormalAttack));
        if (attackType == 2)
            NpcCommon.AttackDetection(transform.position, transform.forward, 140, 3.2f, true, new DamageData(10f, transform.forward * 0.15f, HitType.Heavy, DamageStateInfo.NormalAttack));
    }

    public LayerMask LY;
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
    public void AttackMoveOn(float dis)
    {
        if (!n.collide)
        {
            bAttackMove = true;
            attackMoveDis = dis;
        }
    }
    public void AttackMoveOff()
    {
        bAttackMove = false;
    }
}

