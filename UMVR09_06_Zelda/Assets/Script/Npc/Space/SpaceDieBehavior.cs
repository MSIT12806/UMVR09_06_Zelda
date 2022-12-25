using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceDieBehavior : StateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //ObjectManager2.myCamera.SetDefault();
        //ObjectManager2.myCamera.m_StareTarget[1] = null;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var f = animator.GetLayerWeight(0);
        Debug.Log(f);
        animator.SetLayerWeight(0, f + 0.01f);
    }
}
