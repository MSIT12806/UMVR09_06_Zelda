using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsaoHurtBehavior : StateMachineBehaviour
{
    bool awake = false;
    IKController ik;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //awake
        if (awake == false)
        {
            ik = animator.GetComponent<IKController>();
            //...

            awake = true;
        }

    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ik.IkActive = false;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ik.IkActive = true;
    }
}
