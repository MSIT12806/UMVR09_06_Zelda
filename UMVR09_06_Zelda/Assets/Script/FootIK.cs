using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIK : MonoBehaviour
{
    //左腳
    public Transform LeftFootObj = null;
    //右腳
    public Transform RightFootObj = null;

    //左腳目標
    public Transform LeftFootTarget = null;
    //右腳目標
    public Transform RightFootTarget = null;

    public float Y_Up = 1.5f;
    public float Weight_Down = 0.0f;

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
        FootUp();

        //如果IK沒啟動，則把控制器附上動畫本身的值

        if (IkActive == false)
        {
            
            if (LeftFootObj != null)
            {
                LeftFootObj.position = avatar.GetIKPosition(AvatarIKGoal.LeftFoot);
                LeftFootObj.rotation = avatar.GetIKRotation(AvatarIKGoal.LeftFoot);
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
            avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, Weight_Down);
            avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, Weight_Down);
            avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, Weight_Down);
            avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, Weight_Down);

            if (LeftFootObj != null)
            {
                LeftFootObj.position = LeftFootTarget.position;
                Vector3 vecL = LeftFootObj.localPosition;
                if (vecL.y < Y_Up) vecL.y = Y_Up;
                LeftFootObj.localPosition = vecL;
                avatar.SetIKPosition(AvatarIKGoal.LeftFoot, LeftFootObj.position);
                //avatar.SetIKRotation(AvatarIKGoal.LeftFoot, LeftFootObj.rotation);
            }
            if (RightFootObj != null)
            {
                RightFootObj.position = RightFootTarget.position;
                Vector3 vecR = RightFootObj.localPosition;
                if (vecR.y < Y_Up) vecR.y = Y_Up;
                RightFootObj.localPosition = vecR;
                avatar.SetIKPosition(AvatarIKGoal.RightFoot, RightFootObj.position);
                //avatar.SetIKRotation(AvatarIKGoal.RightFoot, RightFootObj.rotation);
            }
        }

        //不啟動IK
        else
        {
            avatar.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
            avatar.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
            avatar.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
            avatar.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
        }
    }
    void FootUp()
    {
        var stateInfo = avatar.GetCurrentAnimatorStateInfo(0);
        float t = stateInfo.normalizedTime;

        if (stateInfo.IsName("Attack01_SwordAndShiled"))
        {
            SetWeight_Down(1); 
        }

    }

    public void SetWeight_Down( float val)
    {
        Weight_Down = Mathf.Lerp(Weight_Down, val, 0.05f).AlmostEqual(val, 0.01f);
    }
}
