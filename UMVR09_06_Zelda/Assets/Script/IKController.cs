using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    //�Y
    public Transform LookAtObj = null;
    //����
    public Transform BodyObj = null;

    //����y
    public Transform LeftElbowObj = null;
    //�k��y
    public Transform RightElbowObj = null;
    //����
    public Transform LeftHandObj = null;
    //�k��
    public Transform RightHandObj = null;

    //�����\
    public Transform LeftKneeObj = null;
    //�k���\
    public Transform RightKneeObj = null;
    //���}
    public Transform LeftFootObj = null;
    //�k�}
    public Transform RightFootObj = null;

    public float LookAtWeight = 1.0f;
    public float Weight = 1.0f;


    //�ϥ�IK
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
        //�p�GIK�S�ҰʡA�h�ⱱ����W�ʵe��������

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

    //IK�ʵe����M�Ψ��
    private void OnAnimatorIK(int layerIndex)
    {
        //avator���ūh��^
        if (avatar == null) 
            return;

        //�Ұ�IK
        //1.�U�����v��1.0f
        //2.�U�����m���
        //3.����������
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

        //���Ұ�IK
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
