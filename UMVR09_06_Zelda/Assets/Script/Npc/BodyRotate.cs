using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRotate : MonoBehaviour
{
    // Start is called before the first frame update
    public float degree1 = 66.64f;//66.64
    //public float degree2 = 0.0f;
    //public float degree3 = 0.0f;
    //public float degree4 = 0.0f;
    public Transform rightArm;
    public Transform rightForeArm;
    public Transform rightHand;
    public Vector3 rightArmRotate;
    public Vector3 rightForeArmRotate;
    public Vector3 rightHandRotate;
    //public Transform rotatePart2;
    //public Transform rotatePart3;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        rightArm = transform.FindAnyChild<Transform>("RightArm");
        rightForeArm = transform.FindAnyChild<Transform>("RightForeArm");
        rightHand = transform.FindAnyChild<Transform>("RightHand");
    }

    // Update is called once per frame
    void Update()
    {



    }
    private void LateUpdate()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        var f = animator.GetFloat("Forward");
        //if (f > 0.25f && stateInfo.IsName("Grounded"))
        {
            rightArm.Rotate(rightArm.forward, rightArmRotate.z);//z
            rightArm.Rotate(rightArm.up, rightArmRotate.y);//y
            rightArm.Rotate(rightArm.right, rightArmRotate.x);//x

            rightForeArm.Rotate(rightForeArm.forward, rightForeArmRotate.z);//z
            rightForeArm.Rotate(rightForeArm.up, rightForeArmRotate.y);//y
            rightForeArm.Rotate(rightForeArm.right, rightForeArmRotate.x);//x

            rightHand.Rotate(rightHand.forward, rightHandRotate.z);//z
            rightHand.Rotate(rightHand.up, rightHandRotate.y);//y
            rightHand.Rotate(rightHand.right, rightHandRotate.x);//x
        }
    }
}
