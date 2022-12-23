using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ron;

public class SpaceSkill010Behavior : StateMachineBehaviour
{
    Transform target;
    Transform selfTransform;
    SpaceManager space;
    bool canAttack;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        canAttack = true;
        space = animator.transform.GetComponent<SpaceManager>();
        space.InSkill1State = true;
        selfTransform = animator.transform;
        target = ObjectManager2.MainCharacter.transform;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (Once.IcePosision != Vector3.zero)
        //{
        //    float dis = (Once.IcePosision - selfTransform.position).magnitude;
        //    if(dis <= 3.5)
        //    {
        //        Once.IceDestroyTime = 0;
        //        animator.Play("GetHit");
        //    }
        //}

    }
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float dis = (target.transform.position - selfTransform.position).magnitude;

        if (canAttack)
        {
            if (dis > 6)
            {
                space.FaceTarget(target.position, selfTransform, 50);
            }
            if (dis < 2)//&& canAttack
            {
                canAttack = false;
                NpcCommon.AttackDetection("", selfTransform.position, selfTransform.forward, 360, 2, true, new DamageData(80, selfTransform.forward * 0.6f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
                animator.SetTrigger("Skill01HitTarget");
                foreach(var i in space.EffectPlaying)
                {
                    i.Stop();
                    i.Clear();
                }
                //Transform FX = selfTransform.FindAnyChild<Transform>("FX_SpaceSkill01");
                //Transform F = FX.Find("Glow_Ground_Skill01");
                //F.gameObject.SetActive(false);
                //F.GetComponent<ParticleSystem>().Stop();

                //selfTransform.FindAnyChild<ParticleSystem>("Glow_Ground_Skill01").Stop();
            }
        }
        selfTransform.Translate(0, 0, 0.2f);



        if (Once.IcePosision != Vector3.zero)
        {
            if ((Once.IcePosision - selfTransform.position).magnitude <= 3.5)
            {
                Once.IceDestroyTime = 0f;
                space.ShowWeakTime = 5;
                animator.Play("GetHit");
                foreach(var i in space.EffectPlaying)
                {
                    Debug.Log(i.name);
                    i.Stop();
                    i.Clear();
                }
                Debug.Log("innnnnnnnnnnnnnnnnnnnn");
            }
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        space.InSkill1State = false;
    }
}
