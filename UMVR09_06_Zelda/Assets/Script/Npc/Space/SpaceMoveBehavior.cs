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
            target = ObjectManager2.MainCharacter;
            selfTransform = animator.transform;
            awake = true;
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        inStateTime += Time.deltaTime;
        if (inStateTime >= 0.4f)
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
    ///  把順移的腳本寫在這兒
    /// </summary>
    public void Teleport()
    {
        //目標：
        //1. 不能順移出場地
        //2. 不能卡住或無限墜落
        //3. 不能讓 Lico 打不到的位置
        Vector3 NewPos; 
        NewPos = selfTransform.position;
        NewPos.x = RandomFloat(TeleportSpace.Point4.x ,TeleportSpace.Point2.x);
        NewPos.z = RandomFloat(TeleportSpace.Point4.z, TeleportSpace.Point2.z);

        Vector3.Lerp(selfTransform.position, NewPos, 1f);
        //selfTransform.position = NewPos;

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
