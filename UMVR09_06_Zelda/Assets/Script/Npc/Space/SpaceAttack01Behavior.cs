using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceAttack01Behavior : StateMachineBehaviour
{
    Transform target;
    Transform selfTransform;
    SpaceManager space;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space = animator.transform.GetComponent<SpaceManager>();
        selfTransform = animator.transform;
        target = ObjectManager2.MainCharacter.transform;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float dis = (target.position - selfTransform.position).magnitude;
        if (dis < 3.5f)
            selfTransform.RotateAround(TeleportSpace.Center.position, TeleportSpace.Center.up, Time.deltaTime * 160);
        space.FaceTarget(target,selfTransform,15);
    }
}
