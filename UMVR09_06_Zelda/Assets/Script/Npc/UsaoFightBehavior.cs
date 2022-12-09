using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsaoFightBehavior : StateMachineBehaviour
{
    float dazeSeconds;
    Transform target;
    DragonManager manager;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = ObjectManager.MainCharacter;
        var ik = animator.transform.GetComponent<IKController>();
        manager = animator.transform.GetComponent<DragonManager>();
        ik.enabled = true;
        ik.LookAtObj = target.FindAnyChild<Transform>("Head");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (manager.Hp <= 0) animator.Play("Die");
        AiStateCommon.Turn(animator.transform, ObjectManager.MainCharacter.position - animator.transform.position);
        if (dazeSeconds > 0)
        {
            dazeSeconds -= Time.deltaTime;
        }
        var distance = Vector3.Distance(target.position, animator.transform.position);
        //還差衝鋒
        if (distance <= 3f)
        {
            animator.SetTrigger("TailHit");
        }
        if (distance <= 8f)
        {
            animator.SetTrigger("FireHit");
        }
        else
        {
            animator.SetBool("Move", true);
        }
    }
    void RefreshDazeTime()
    {
        dazeSeconds = UnityEngine.Random.Range(3, 6);
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
