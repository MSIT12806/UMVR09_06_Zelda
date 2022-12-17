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
    float movePerframe = 0.4f;

    Vector3 MoveTo;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (awake == false)
        {
            target = ObjectManager2.MainCharacter;
            selfTransform = animator.transform;
            awake = true;
        }
        MoveTo = GetWalkPoint();
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (selfTransform.position == MoveTo)
        {
            animator.SetTrigger("MoveFinish");
        }
        else if(selfTransform.position != MoveTo)
        {
            selfTransform.LookAt(MoveTo);
        }
        Vector3 dir = MoveTo - selfTransform.position;
        float dis = (MoveTo - selfTransform.position).magnitude;
        selfTransform.Translate(0, 0, movePerframe);
        if(dis <= movePerframe)
        {
            selfTransform.position = MoveTo;
        }
        //inStateTime += Time.deltaTime;
        //if (inStateTime >= 0.4f)
        //{
        //    Teleport();
        //    inStateTime = 0;
        //}

    }

    public Vector3 GetWalkPoint()
    {
        Vector3 NewPos;
        NewPos = selfTransform.position;
        NewPos.x = RandomFloat(TeleportSpace.Point4.x, TeleportSpace.Point2.x);
        NewPos.z = RandomFloat(TeleportSpace.Point4.z, TeleportSpace.Point2.z);
        float dis = (NewPos - selfTransform.position).magnitude;
        if (dis < 5f || dis > 10 )
            GetWalkPoint();
        if ((target.position - selfTransform.position).magnitude > 10 && (target.position - selfTransform.position).magnitude < 1)
            GetWalkPoint();
        return NewPos;
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
