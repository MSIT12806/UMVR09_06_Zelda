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
    bool fightState { get => (int)state.gameState == 2 && manager.Show; }

    float flyStateWeight;
    public Npc npc;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (npc == null) npc = animator.GetComponent<Npc>();

        if (animator.name == "Blue Variant 2") return;
        target = ObjectManager.MainCharacter;
        manager = animator.transform.GetComponent<DragonManager>();
        state = target.GetComponent<PicoState>();
        head = animator.transform.FindAnyChild<Transform>("Head");
        animator.SetBool("Fly", false);
        manager.DizzyEnd();
        RefreshDazeTime();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.name == "Blue Variant 2") return;
        if (manager.Hp <= 0)
        {
            manager.Die();

        }

        if (fightState)
        {
            if (flyStateWeight > 0.3)
            {
                animator.SetBool("Fly", true);
                return;
            }



            if (dazeSeconds > 0)
            {
                dazeSeconds -= Time.deltaTime;
                return;
            }
            // do attack
            var distance = Vector3.Distance(target.position, animator.transform.position);
            if (manager.Hp < manager.MaxHp / 2 && UnityEngine.Random.value > 0.5)
            {
                animator.SetTrigger("Sprint");
            }

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
        dazeSeconds = UnityEngine.Random.Range(2, 4);
        flyStateWeight = UnityEngine.Random.value;
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.name == "Blue Variant 2") return;
        if (fightState)
        {
            if (npc.PauseTime >= 0) return;
            AiStateCommon.Turn(animator.transform, target.position - animator.transform.position);
            AiStateCommon.Look(head, target);

        }
    }

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
