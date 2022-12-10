using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class DragonChaseBehavior : StateMachineBehaviour
{
    Transform target;
    DragonManager manager;
    Npc npc;
    Vector3 targetPoint;
    float lastDistance = float.MaxValue - 100;
    public  float speed ;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = ObjectManager.MainCharacter;
        manager = animator.transform.GetComponent<DragonManager>();
        npc = animator.transform.GetComponent<Npc>();
        targetPoint = manager.ArrivePoint;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lastDistance = float.MaxValue - 100;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (manager.Hp <= 0) animator.Play("Die");
        var d = Vector3.Distance(targetPoint, animator.transform.position);
        if (lastDistance - d > 0.05f)
        {
            Debug.Log(targetPoint);
            animator.transform.position += animator.transform.forward * speed;
            lastDistance = d;
        }
        else
        {
            animator.SetBool("Move", false);
        }
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
