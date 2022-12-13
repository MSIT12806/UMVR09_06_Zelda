using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 處理對峙時移動的碰撞問題
/// </summary>
public class UsaoWalkBehavior : StateMachineBehaviour
{
    Npc npc;
    bool awake = false;
    IKController ik;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //awake
        if (awake == false)
        {
            ik = animator.GetComponent<IKController>();
            npc = animator.GetComponent<Npc>();
            //...

            awake = true;
        }

        ik.LookAtObj = ObjectManager.MainCharacterHead;
        ik.IkActive = true;
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (npc.collide)
        {
            animator.applyRootMotion = false;
        }
        else
        {
            animator.applyRootMotion = true;
            AiStateCommon.Turn(animator.transform, ObjectManager.MainCharacter.position - animator.transform.position);
        }
    }
}
