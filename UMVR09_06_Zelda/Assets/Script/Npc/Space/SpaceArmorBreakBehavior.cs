using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceArmorBreakBehavior : StateMachineBehaviour
{
    SpaceManager space;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space = animator.transform.GetComponent<SpaceManager>();
        space.dizzy = true;
        space.CanGetHit = false;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space.ArmorBreakTime -= Time.deltaTime;

        if(space.ArmorBreakTime <= 0)
        {
            animator.Play("standing_idle");
            space.Armor = space.MaxArmor;
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space.dizzy = false;
        space.ArmorBreakTime = 7;
        //®zÂIUIÁôÂÃ
    }
}
