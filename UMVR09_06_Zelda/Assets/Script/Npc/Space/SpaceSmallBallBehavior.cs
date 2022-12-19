using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ron;

public class SpaceSmallBallBehavior : StateMachineBehaviour
{
    SpaceManager manager;
    Transform target;
    bool awake;
    float dazeSeconds;
    float weight;

    GameObject SmallBalls;
    Transform selfTransform;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SmallBalls = animator.transform.FindAnyChild<Transform>("SpaceWeapons").gameObject;
        SmallBalls.SetActive(true);
        target = ObjectManager2.MainCharacter;

        if (awake == false)
        {
            manager = animator.GetComponent<SpaceManager>();
            awake = true;
        }
        selfTransform = animator.transform;
        dazeSeconds = UnityEngine.Random.value * 3;
        weight = UnityEngine.Random.value;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float dis = (target.position - selfTransform.position).magnitude;//lico貼太進會移動
        if (dis < 3.5f)
            animator.transform.RotateAround(TeleportSpace.Center.position, TeleportSpace.Center.up, Time.deltaTime * 160);
    }
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        AiStateCommon.Turn(animator.transform, (target.position - animator.transform.position).normalized);
        foreach (var item in manager.smallBalls)
        {
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SmallBalls.SetActive(false);
    }
}
