using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceFightBehavior : StateMachineBehaviour
{
    SpaceManager manager;
    Transform target;
    bool awake;
    float dazeSeconds;
    float weight;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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
        dazeSeconds -= Time.deltaTime;
        if(dazeSeconds <= 0)
        {
            animator.SetTrigger("Move");
        }

        var distance = Vector3.Distance(animator.transform.position, target.position);

        if (distance <= 15 && weight > 0.8)
        {
            animator.SetTrigger("Attack04");//15m內都可以追蹤小球
        }
        else if(distance<=10 && weight > 0.8)
        {
            animator.SetTrigger("Attack03");//10m內可以發射氣功
        }
        else if (distance <= 6)
        {
            animator.SetTrigger("Attack02");//6m內爆炸？
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
