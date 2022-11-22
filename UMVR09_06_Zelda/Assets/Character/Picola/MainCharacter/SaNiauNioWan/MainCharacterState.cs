using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterState : MonoBehaviour
{
    public GameObject Sword;
    public Animator animator;
    public AnimatorStateInfo currentAnimation;
    private float fTimer = 0f;

    Vector3 newPos;
    bool frontMove = false;
    float time = 0f;
    public Transform newPlace;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 newPos = transform.position;

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
            if (Input.GetMouseButtonDown(0))
                LeftMouseClick();
            if (Input.GetMouseButtonDown(1))
                RightMouseClick();
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            fTimer += Time.deltaTime;
            animator.SetFloat("dodge", fTimer);
        }
        else
        {
            fTimer = 0f;
            animator.SetFloat("dodge", fTimer);
        }

        var a = this.GetComponent<IKController>();
        if (currentAnimation.IsName("Fast run"))
        {
            a.IkActive = false;
            Sword.SetActive(false);
        }
        else
        {
            a.IkActive = true;
            Sword.SetActive(true);
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

        if (Input.GetKeyDown(KeyCode.LeftControl) && (currentAnimation.IsName("Grounded") || currentAnimation.IsName("Front Dodge")))
        {
            frontMove = true;
        }
        if (frontMove)
        {
            time += Time.deltaTime;
        }
        if (0f < time && time < 0.21f)
        {
            print(transform.forward);
            transform.Translate( new Vector3(0f,0f,1f) * 0.15f);
        }
        else if (time > 0.4f)
        {
            frontMove = false;
            time = 0f;
        }
        Debug.DrawLine(transform.position,transform.position+ transform.forward*10f);

        var IK = GetComponent<IKController>();
        if (currentAnimation.IsName("Attack01 2") || currentAnimation.IsName("Fast run"))
            IK.IkActive = false;
        else
            IK.IkActive = true;


        //if (Input.GetMouseButtonDown(0))
        //{
        //    //transform.position = transform.position + transform.forward * 10f;
        //    transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward.normalized * 2f, 0.1f);
        //}
    }
    public virtual void LeftMouseClick()
    {
        animator.SetTrigger("attack01");
    }
    public virtual void RightMouseClick()
    {
        if (currentAnimation.IsName("Attack01") || currentAnimation.IsName("Attack01 0") || currentAnimation.IsName("Attack01 1") || currentAnimation.IsName("Attack01 2"))
            animator.SetBool("attack02",true);
        else
            animator.SetBool("attack02", false);
    }

    public void ForwardMove()
    {
        //設定true false開關外面主程式來位移------------------------------------------------------------------------------
        transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward.normalized * 1f, 1f);
        //print(transform.position);
        //transform.position = transform.position + transform.forward * 100f;
        //print(transform.position);

    }

    public void AttackSpeedChange(float f)
    {
        animator.SetFloat("attackSpeed",f*1.5f);
        //print(231321213);
    }
}

