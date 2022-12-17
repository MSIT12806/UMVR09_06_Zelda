using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSkill010Behavior : StateMachineBehaviour
{
    Transform target;
    Transform selfTransform;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        selfTransform = animator.transform;
        target = ObjectManager2.MainCharacter.transform;
        UiManager.singleton.ShowSikaTip("ItemIceTips");
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float dis = (target.transform.position - selfTransform.position).magnitude;
        if (dis < 2)
            NpcCommon.AttackDetection("", selfTransform.position, selfTransform.forward, 90, 2, true, new DamageData(80, selfTransform.forward * 0.6f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
        animator.transform.LookAt(target);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UiManager.singleton.HideTip();
    }
}
