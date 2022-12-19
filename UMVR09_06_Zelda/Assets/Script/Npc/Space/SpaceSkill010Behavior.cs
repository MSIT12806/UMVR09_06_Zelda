using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSkill010Behavior : StateMachineBehaviour
{
    Transform target;
    Transform selfTransform;
    SpaceManager space;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space = animator.transform.GetComponent<SpaceManager>();
        space.InSkill1State = true;
        selfTransform = animator.transform;
        target = ObjectManager2.MainCharacter.transform;
        UiManager.singleton.ShowSikaTip("ItemIceTips");
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float dis = (target.transform.position - selfTransform.position).magnitude;
        if (dis < 2)
        {
            NpcCommon.AttackDetection("", selfTransform.position, selfTransform.forward, 90, 2, true, new DamageData(80, selfTransform.forward * 0.6f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
            animator.SetTrigger("Skill01HitTarget");
        }
        //if (dis > 4)
        space.FaceTarget(target.position, selfTransform, 360);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UiManager.singleton.HideTip();
        space.InSkill1State = false;
    }
}
