using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMoveBehavior : StateMachineBehaviour
{
    Transform selfTransform;
    Transform target;
    bool awake;
    bool TpTrigger = false;
    float inStateTime = 0;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (awake == false)
        {
            target = ObjectManager.MainCharacter;
            selfTransform = animator.transform;
            awake = true;
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        inStateTime += Time.deltaTime;
        if (inStateTime > 0.5f)
        {
            Teleport();
            inStateTime = 0;
        }
        //if (TpTrigger)
        //{
        //    Teleport();
        //    TpTrigger = false;
        //}
    }

    /// <summary>
    ///  �ⶶ�����}���g�b�o��
    /// </summary>
    public void Teleport()
    {
        //�ؼСG
        //1. ���බ���X���a
        //2. ����d��εL���Y��
        //3. ������ Lico �����쪺��m
        Vector3 NewPos;
        NewPos = selfTransform.position;
        NewPos.x = RandomFloat(TeleportSpace.Point4.x ,TeleportSpace.Point2.x);
        NewPos.z = RandomFloat(TeleportSpace.Point4.z, TeleportSpace.Point2.z);

        selfTransform.position = NewPos;

    }
    public float RandomFloat(float min, float max)
    {
        System.Random random = new System.Random();
        double val = (random.NextDouble() * (max - min) + min);
        return (float)val;
    }
    public void CanTp()
    {
        TpTrigger = true;
        Debug.Log("hiiiiiii");
    }
}
