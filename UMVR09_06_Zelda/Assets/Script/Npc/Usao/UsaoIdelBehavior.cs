using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsaoIdelBehavior : StateMachineBehaviour
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

        ik.LookAtObj = ObjectManager.MainCharacterHead;
        ik.IkActive = false;
    }

}
