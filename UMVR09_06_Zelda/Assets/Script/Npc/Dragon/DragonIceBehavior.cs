using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonIceBehavior : StateMachineBehaviour
{
    Transform target;
    DragonManager manager;
    Transform head;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = ObjectManager.MainCharacter;
        manager = animator.transform.GetComponent<DragonManager>();
        manager.sprinting = true;
        manager.canBeKnockedOut = true;
        head = animator.transform.FindAnyChild<Transform>("Head");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        manager.sprinting = false;
        manager.canBeKnockedOut = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (manager.Hp <= 0) manager.Die();
        AiStateCommon.Turn(animator.transform, ObjectManager.MainCharacter.position - animator.transform.position);
        Turn(head, ObjectManager.MainCharacter.position - head.transform.position);
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    bool Turn(Transform body, Vector3 direction)
    {
        var degree = Vector3.SignedAngle(-body.right.WithY(), direction.WithY(), Vector3.up);
        if (degree < -1)
        {
            body.Rotate(Vector3.up, -2);
            return true;

        }
        else if (degree > 1)
        {
            body.Rotate(Vector3.up, 2);
            return true;
        }

        return false;
    }
}
