using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonShowToolBehavior : StateMachineBehaviour
{
    Npc npc;
    public string SikaTool;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UiManager.singleton.ShowSikaTip(SikaTool);
        npc = animator.GetComponent<Npc>();
    }
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (npc.PauseTime <= 0)
            animator.transform.LookAt(ObjectManager.MainCharacter.position);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UiManager.singleton.HideTip();
    }
}
