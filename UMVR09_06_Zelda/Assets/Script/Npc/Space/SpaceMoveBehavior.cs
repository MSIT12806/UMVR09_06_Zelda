using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMoveBehavior : StateMachineBehaviour
{
    Transform target;
    bool awake;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (awake == false)
        {
            target = ObjectManager.MainCharacter;
            awake = true;
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
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
    }
}
