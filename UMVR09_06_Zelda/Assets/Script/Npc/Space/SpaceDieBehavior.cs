using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceDieBehavior : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var f = animator.GetLayerWeight(0);
        Debug.Log(f);
        animator.SetLayerWeight(0, f + 0.01f);
    }
}
