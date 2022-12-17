using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceSmallBallBehavior : StateMachineBehaviour
{
    SpaceManager manager;
    Transform target;
    bool awake;
    float dazeSeconds;
    float weight;
    GameObject SmallBalls;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SmallBalls = animator.transform.GetChild(0).GetChild(1).gameObject;
        SmallBalls.SetActive(true);
        if (awake == false)
        {
            manager = animator.GetComponent<SpaceManager>();
            target = ObjectManager2.MainCharacter;
            awake = true;
        }
        dazeSeconds = UnityEngine.Random.value * 3;
        weight = UnityEngine.Random.value;
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        AiStateCommon.Turn(animator.transform, (target.position - animator.transform.position).normalized);
        foreach (var item in manager.smallBalls)
        {
            item.Attack();
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
