using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    //頭
    public Transform LookAtObj = null;
    //身體
    public Transform BodyObj = null;

    //左手肘
    public Transform LeftElbowObj = null;
    //右手肘
    public Transform RightElbowObj = null;
    //左手
    public Transform LeftHandObj = null;
    //右手
    public Transform RightHandObj = null;

    //左膝蓋
    public Transform LeftKneeObj = null;
    //右膝蓋
    public Transform RightKneeObj = null;
    //左腳
    public Transform LeftFootObj = null;
    //右腳
    public Transform RightFootObj = null;

    public float LookAtWeight = 1.0f;
    public float Weight = 1.0f;


    //使用IK
    public bool IkActive = false;

    private Animator avatar;

    // Start is called before the first frame update
    void Start()
    {
        avatar = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //如果IK沒啟動，則把控制器附上動畫本身的值

        if (IkActive == false) 
        {
            if (LookAtObj != null)
            {
                LookAtObj.position = avatar.bodyPosition + avatar.bodyRotation * new Vector3(0, 0.5f, 0);
            }
            if (BodyObj != null) 
            {
                BodyObj.position = avatar.bodyPosition;
                BodyObj.rotation = avatar.bodyRotation;
            }
            if (LeftElbowObj != null)
            {
                LeftElbowObj.position = avatar.GetIKHintPosition(AvatarIKHint.LeftElbow);
            }
            if (RightElbowObj != null)
            {
                RightElbowObj.position = avatar.GetIKHintPosition(AvatarIKHint.RightElbow);
            }
            if (LeftHandObj != null)
            {
                LeftElbowObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftHand);
                LeftElbowObj.rotation = avatar.GetIKRotation(AvatarIKGoal.LeftHand);
            }
            if (RightHandObj != null)
            {
                RightHandObj.position = avatar.GetIKPosition(AvatarIKGoal.RightHand);
                RightHandObj.rotation = avatar.GetIKRotation(AvatarIKGoal.RightHand);
            }

            if (LeftKneeObj != null)
            {
                LeftKneeObj.position = avatar.GetIKHintPosition(AvatarIKHint.LeftKnee);
            }
            if (RightKneeObj != null)
            {
                RightKneeObj.position = avatar.GetIKHintPosition(AvatarIKHint.RightKnee);
            }
            if (LeftFootObj != null)
            {
                LeftElbowObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftFoot);
                LeftElbowObj.rotation = avatar.GetIKRotation(AvatarIKGoal.LeftFoot);
            }
            if (RightFootObj != null)
            {
                RightFootObj.position = avatar.GetIKPosition(AvatarIKGoal.RightFoot);
                RightFootObj.rotation = avatar.GetIKRotation(AvatarIKGoal.RightFoot);
            }
        }
    }

    //IK動畫控制專用函數
    private void OnAnimatorIK(int layerIndex)
    {
        //avator為空則返回
        if (avatar == null) 
            return;

        //啟動IK
        //1.各部位權重1.0f
        //2.各部位位置賦值
        //3.部分旋轉賦值
        if (IkActive) 
        {
            avatar.SetLookAtWeight(LookAtWeight, 0.3f, 0.6f, 1.0f, 0.5f);

            avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, Weight);
            avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, Weight);
            avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, Weight);
            avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, Weight);

            avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, Weight);
            avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, Weight);
            avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, Weight);
            avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, Weight);

            avatar.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, Weight);
            avatar.SetIKHintPositionWeight(AvatarIKHint.RightElbow, Weight);
            avatar.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, Weight);
            avatar.SetIKHintPositionWeight(AvatarIKHint.RightKnee, Weight);

            if (LookAtObj != null) 
            {
                avatar.SetLookAtPosition(LookAtObj.position);
            }

            if(BodyObj != null)
            {
                avatar.bodyPosition = BodyObj.position;
                avatar.bodyRotation = BodyObj.rotation;
            }
            if (LeftElbowObj != null) 
            {
                avatar.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowObj.position);
            }
            if (RightElbowObj != null)
            {
                avatar.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowObj.position);
            }
            if (LeftHandObj != null) 
            {
                avatar.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandObj.position);
                avatar.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandObj.rotation);
            }
            if (RightHandObj != null)
            {
                avatar.SetIKPosition(AvatarIKGoal.RightHand, RightHandObj.position);
                avatar.SetIKRotation(AvatarIKGoal.RightHand, RightHandObj.rotation);
            }

            if (LeftKneeObj != null)
            {
                avatar.SetIKHintPosition(AvatarIKHint.LeftKnee, LeftKneeObj.position);
            }
            if (RightKneeObj != null)
            {
                avatar.SetIKHintPosition(AvatarIKHint.RightKnee, RightKneeObj.position);
            }
            if (LeftFootObj != null)
            {
                avatar.SetIKPosition(AvatarIKGoal.LeftFoot, LeftFootObj.position);
                avatar.SetIKRotation(AvatarIKGoal.LeftFoot, LeftFootObj.rotation);
            }
            if (RightFootObj != null)
            {
                avatar.SetIKPosition(AvatarIKGoal.RightFoot, RightFootObj.position);
                avatar.SetIKRotation(AvatarIKGoal.RightFoot, RightFootObj.rotation);
            }
        }

        //不啟動IK
        else 
        {
            avatar.SetLookAtWeight(0.0f);

            avatar.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
            avatar.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);

            avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);

            avatar.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 0);
            avatar.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 0);

            avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
            avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
            avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
            avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);

        }
    }

}
