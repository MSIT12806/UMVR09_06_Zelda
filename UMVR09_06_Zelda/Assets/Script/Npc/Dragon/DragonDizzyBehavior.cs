using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class DragonDizzyBehavior : StateMachineBehaviour
{
    DragonManager manager;
   
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        manager = animator.GetComponent<DragonManager>();
        manager.DizzyStart();
    }

    
}
