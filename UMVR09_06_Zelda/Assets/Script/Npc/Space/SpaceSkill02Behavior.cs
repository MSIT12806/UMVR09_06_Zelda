using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSkill02Behavior : StateMachineBehaviour
{
    Transform target;
    SpaceManager space;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space = animator.transform.GetComponent<SpaceManager>();
        space.InSkill2State = true;
        target = ObjectManager2.MainCharacter.transform;
        UiManager.singleton.ShowSikaTip("ItemBombTips");
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.LookAt(target);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space.InSkill2State = false;
    }
}
