using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceAttack02Behavior : StateMachineBehaviour
{
    Transform target;
    SpaceManager space;
    Transform selfTransform;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space = animator.transform.GetComponent<SpaceManager>();
        target = ObjectManager2.MainCharacter.transform;
        selfTransform = animator.transform;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space.FaceTarget(target, selfTransform,15);

    }
}
