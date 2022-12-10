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
    Vector3 flyPoint;
    float lastDistance = float.MaxValue - 100;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = ObjectManager.MainCharacter;
        manager = animator.transform.GetComponent<DragonManager>();
        npc = animator.transform.GetComponent<Npc>();
        flyPoint = manager.ArrivePoint;
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
        var d = Vector3.Distance(flyPoint, animator.transform.position);
        if (lastDistance - d > 0.2f)
        {
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
