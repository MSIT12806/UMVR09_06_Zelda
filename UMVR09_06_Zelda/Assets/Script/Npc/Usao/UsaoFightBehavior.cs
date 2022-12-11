using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsaoFightBehavior : StateMachineBehaviour
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
        ik.IkActive = true;
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AiStateCommon.Turn(animator.transform, ObjectManager.MainCharacter.position - animator.transform.position);
    }
}
