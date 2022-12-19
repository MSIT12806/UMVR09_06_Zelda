using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ron;

public class SpaceAttack04Behavior : StateMachineBehaviour
{
    Transform target;
    Transform selfTransform;
    SpaceManager space;
    Transform balls;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        selfTransform = animator.transform;
        balls = selfTransform.FindAnyChild<Transform>("SpaceWeapons");
        balls.gameObject.SetActive(true);
        space = selfTransform.GetComponent<SpaceManager>();
        target = ObjectManager2.MainCharacter.transform;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space.smallBallsAroundBody.ForEach(i => i.SetActive(true));
        balls.gameObject.SetActive(false);
    }

}
