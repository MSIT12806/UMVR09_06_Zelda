using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceAttack03Behavior : StateMachineBehaviour
{
    Transform target;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        target = ObjectManager2.MainCharacter.transform;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.LookAt(target);
    }
}
