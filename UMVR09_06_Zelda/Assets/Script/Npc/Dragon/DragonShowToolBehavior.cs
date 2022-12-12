using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonShowToolBehavior : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UiManager.singleton.ShowSikaTip("ItemBombTips");
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UiManager.singleton.HideTip();
    }
}
