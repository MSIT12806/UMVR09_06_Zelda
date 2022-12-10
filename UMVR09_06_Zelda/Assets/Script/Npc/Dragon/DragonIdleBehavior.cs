using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonIdleBehavior : StateMachineBehaviour
{
    Transform target;
    DragonManager manager;
    PicoState state;
    float dazeSeconds;
    Transform head;
    bool fightState { get => (int)state.gameState == 2; }
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = ObjectManager.MainCharacter;
        manager = animator.transform.GetComponent<DragonManager>();
        state = target.GetComponent<PicoState>();
        head = animator.transform.FindAnyChild<Transform>("Head");

        RefreshDazeTime();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (manager.Hp <= 0)
        {
            animator.Play("Die");
        }

        if (fightState)
        {
            if (manager.Hp > manager.MaxHp / 2)
            {
                animator.SetBool("Fly", true);
                return;
            }

            AiStateCommon.Turn(animator.transform, target.position - animator.transform.position);
            AiStateCommon.Look(head, target);

            if (dazeSeconds > 0)
            {
                dazeSeconds -= Time.deltaTime;
                return;
            }
            // do attack
            var distance = Vector3.Distance(target.position, animator.transform.position);
            //還差衝鋒...然後要用隨機來處理
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
                manager.ArrivePoint = target.position + (animator.transform.position - target.position).normalized * 1.5f;
                animator.SetBool("Move", true);
            }
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
