using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceWeakBehavior : StateMachineBehaviour
{
    SpaceManager manager;
    Transform target;
    bool awake;
    float weakSeconds;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (awake == false)
        {
            manager = animator.GetComponent<SpaceManager>();
            target = ObjectManager2.MainCharacter;
            awake = true;
        }
        weakSeconds = 5;
        //露出弱點
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        weakSeconds -= Time.deltaTime;
        if (weakSeconds <= 0)
        {
            animator.SetTrigger("backToIdle");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //藏起弱點
    }
}
