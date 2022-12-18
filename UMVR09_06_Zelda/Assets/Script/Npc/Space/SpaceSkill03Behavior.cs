using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSkill03Behavior : StateMachineBehaviour
{
    Transform target;
    SpaceManager space;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space = animator.transform.GetComponent<SpaceManager>();
        space.InSkill3State = true;
        target = ObjectManager2.MainCharacter.transform;
        UiManager.singleton.ShowSikaTip("ItemLockTips");
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.LookAt(target);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space.InSkill3State = false;
    }
}
