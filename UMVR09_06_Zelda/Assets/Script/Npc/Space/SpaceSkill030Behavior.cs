using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSkill030Behavior : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UiManager.singleton.ShowSikaTip("ItemIceTips");
    }
}
