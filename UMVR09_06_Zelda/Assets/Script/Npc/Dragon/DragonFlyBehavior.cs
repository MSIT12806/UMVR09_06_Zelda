using Ron;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DragonFlyBehavior : StateMachineBehaviour
{
    Transform target;
    DragonManager manager;
    PicoState state;
    Npc npc;
    float dazeSeconds;
    Transform head;
    bool fightState { get => (int)state.gameState == 2; }

    private Vector3 flyPoint = Vector3.zero;
    bool turnFinish;
    private bool chase;
    private float distance;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = ObjectManager.MainCharacter;
        manager = animator.transform.GetComponent<DragonManager>();
        state = target.GetComponent<PicoState>();
        head = animator.transform.FindAnyChild<Transform>("Head");
        flyPoint = Vector3.zero;
        distance = Vector3.Distance(target.position, animator.transform.position);
        var randomDir = new Vector3(UnityEngine.Random.value - 0.5f, 0, UnityEngine.Random.value - 0.5f).normalized;
        manager.ArrivePoint = flyPoint == Vector3.zero ? target.position + randomDir * 7.5f : flyPoint;
        RefreshDazeTime();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    void RefreshDazeTime()
    {
        dazeSeconds = UnityEngine.Random.Range(3, 6);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        distance = 0;
        chase = false;
        turnFinish = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (manager.Hp <= 0)
        {
            animator.Play("Die");
        }

        if (fightState)
        {

            if (manager.Hp < manager.MaxHp / 2) //血量低於一半，降落
            {
                animator.SetBool("Fly", false);
                return;
            }

            // do attack
            //還差衝鋒
            if (distance > 10 || distance < 5)
            {
                //轉向飛行目標，轉完就切換成飛行模式
                turnFinish = !AiStateCommon.Turn(animator.transform, manager.ArrivePoint - animator.transform.position);
                if (turnFinish)
                {
                    animator.SetBool("Move", true);
                }
            }
            else
            {
                AiStateCommon.Turn(animator.transform, target.position - animator.transform.position);
                AiStateCommon.Look(head, target);
                if (dazeSeconds > 0)
                {
                    dazeSeconds -= Time.deltaTime;
                    return;
                }
                animator.SetTrigger("FireHit");
            }

        }
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Implement code that sets up animation IK (inverse kinematics)
    }
}
