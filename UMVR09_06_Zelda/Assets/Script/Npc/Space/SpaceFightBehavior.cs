using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceFightBehavior : StateMachineBehaviour
{
    SpaceManager manager;
    Transform target;
    bool awake;
    //float dazeSeconds;
    float weight;
    bool move;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (awake == false)
        {
            manager = animator.GetComponent<SpaceManager>();
            target = ObjectManager2.MainCharacter;
            awake = true;
        }
        //dazeSeconds = UnityEngine.Random.value * 3;
        weight = UnityEngine.Random.value;
        Debug.Log(move);

        if (!move)
        {
            float rnd = UnityEngine.Random.value;
            if (rnd <= 0.7)
                animator.SetTrigger("ToMove");
            else
                animator.SetTrigger("ToTeleport");
            return;
        }

        var distance = Vector3.Distance(animator.transform.position, target.position);

        Attack(distance, animator);
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AiStateCommon.Turn(animator.transform, (target.position - animator.transform.position).normalized);
        //dazeSeconds -= Time.deltaTime;
        //if(dazeSeconds <= 0)


    }

    public void Attack(float dis , Animator animator)
    {
        if (dis > 15) animator.SetTrigger("ToMove");


        else if(dis > 10)
        {
            if(weight <= 0.5)
            {
                animator.SetTrigger("Attack01");
            }
            else if (weight <= 1.0)
            {
                animator.SetTrigger("Attack04");
            }
        }

        else if (dis > 7)
        {
            if (weight <= 0.15)
            {
                animator.SetTrigger("Attack04");
            }
            else if (weight <= 0.3)
            {
                animator.SetTrigger("Attack01");
            }
            else if (weight <= 0.53)
            {
                animator.SetTrigger("Skill01");
            }
            else if (weight <= 0.76)
            {
                animator.SetTrigger("Skill02");
            }
            else if (weight <= 1.0)
            {
                animator.SetTrigger("Skill03");
            }
        }

        else if (dis > 4)
        {
            if (weight <= 0.1)
            {
                animator.SetTrigger("Attack04");
            }
            else if (weight <= 0.2)
            {
                animator.SetTrigger("Attack01");
            }
            else if (weight <= 0.35)
            {
                animator.SetTrigger("Skill01");
            }
            else if (weight <= 0.5)
            {
                animator.SetTrigger("Skill02");
            }
            else if (weight <= 0.65)
            {
                animator.SetTrigger("Skill03");
            }
            else if (weight <= 1.0)
            {
                animator.SetTrigger("Attack03");
            }
        }

        else
        {
            if (weight <= 0.05)
            {
                animator.SetTrigger("Attack04");
            }
            else if (weight <= 0.1)
            {
                animator.SetTrigger("Attack01");
            }
            else if (weight <= 0.2)
            {
                animator.SetTrigger("Skill01");
            }
            else if (weight <= 0.3)
            {
                animator.SetTrigger("Skill02");
            }
            else if (weight <= 0.4)
            {
                animator.SetTrigger("Skill03");
            }
            else if (weight <= 0.5)
            {
                animator.SetTrigger("Attack03");
            }
            else if (weight <= 1.0)
            {
                animator.SetTrigger("Attack02");
            }
        }
    }

    public void CanAttackType()
    {

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        move = !move;
    }
}
