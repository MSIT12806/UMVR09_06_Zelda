using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceGetHitBeHavior1 : StateMachineBehaviour
{
    SpaceManager space;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space = animator.transform.GetComponent<SpaceManager>();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(space.ShowWeakTime <= 0)
        {
            animator.Play("standing_idle");
        }
    }

}
