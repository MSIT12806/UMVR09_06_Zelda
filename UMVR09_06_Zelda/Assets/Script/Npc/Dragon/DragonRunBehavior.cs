using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class DragonRunBehavior : StateMachineBehaviour
{
    bool awake;
    Npc npc;

    Vector3 targetPosition;
    Vector3 direction;
    float currentDistance;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (awake == false)
        {
            awake = true;
            npc = animator.GetComponent<Npc>();
        }
        direction = (ObjectManager.MainCharacter.position - animator.transform.position).normalized;
        var distance = Vector3.Distance(ObjectManager.MainCharacter.position, animator.transform.position) + 2f;
        targetPosition = animator.transform.position + direction * distance;
        currentDistance = float.MaxValue;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log(Vector3.Distance(animator.transform.position, ObjectManager.MainCharacter.position));
        var isHit = Vector3.Distance(animator.transform.position, ObjectManager.MainCharacter.position) <= 2f;
        if (isHit)
        {
            NpcCommon.AttackDetection("Dragon", animator.transform.position, animator.transform.forward,35, 2f, false, new DamageData(100, animator.transform.forward, HitType.Heavy, DamageStateInfo.NormalAttack),"Player");
        }
        var newDistance = Vector3.Distance(animator.transform.position, targetPosition);
        if (currentDistance > newDistance && npc.collide==false)
        {
            currentDistance = newDistance;
            animator.transform.position += direction * 0.3f;
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
