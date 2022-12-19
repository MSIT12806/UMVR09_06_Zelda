using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSkill02Behavior : StateMachineBehaviour
{
    Transform target;
    Transform selfTransform;
    SpaceManager space;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        selfTransform = animator.transform;
        space = animator.transform.GetComponent<SpaceManager>();
        space.InSkill2State = true;
        target = ObjectManager2.MainCharacter.transform;
        UiManager.singleton.ShowSikaTip("ItemBombTips");
        space.FaceTarget(target.position, selfTransform, 360);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space.InSkill2State = false;
    }
}
