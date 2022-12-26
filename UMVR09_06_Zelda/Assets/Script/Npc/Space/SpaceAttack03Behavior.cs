using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceAttack03Behavior : StateMachineBehaviour
{
    Transform target;
    Transform selfTransform;
    SpaceManager space;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        selfTransform = animator.transform;
        space = selfTransform.GetComponent<SpaceManager>();
        target = ObjectManager2.MainCharacter.transform;
        space.FaceTarget(target.position, selfTransform,360);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
