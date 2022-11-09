using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineScript : MonoBehaviour
{
    public Animator animator;
    public AnimatorStateInfo asi;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        asi = animator.GetCurrentAnimatorStateInfo(0);
        var tpc = this.GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter>();
        if (asi.IsName("Attack01") || asi.IsName("Attack01 0") || asi.IsName("Attack01 1") || asi.IsName("Attack01 2") || asi.IsName("GetHit") || asi.IsName("Die"))
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

        if (animator.IsInTransition(0) == false)
        {
            if (Input.GetMouseButtonDown(0))
                LeftMouseClick();
            if (Input.GetMouseButtonDown(1))
                RightMouseClick();
        }
    }
    public virtual void LeftMouseClick()
    {
        animator.SetTrigger("attack01");
    }
    public virtual void RightMouseClick()
    {
        animator.SetTrigger("attack02");
    }

}
