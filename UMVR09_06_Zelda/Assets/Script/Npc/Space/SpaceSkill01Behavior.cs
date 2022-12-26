using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSkill01Behavior : StateMachineBehaviour
{
    Transform target;
    Transform selfTransform;
    SpaceManager space;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        selfTransform = animator.transform;
        space = selfTransform.GetComponent<SpaceManager>();
        target = ObjectManager2.MainCharacter.transform;
        UiManager.singleton.ShowSikaTip("ItemIceTips");
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space.FaceTarget(target.position, selfTransform, 15);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        UiManager.singleton.HideTip();
    }
}
