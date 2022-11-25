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
    public Transform rotatePart1;
    //public Transform rotatePart2;
    //public Transform rotatePart3;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {



    }
    private void LateUpdate()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        var f = animator.GetFloat("Forward");
        if (f > 0.25f && stateInfo.IsName("Grounded"))
        {
            rotatePart1.Rotate(rotatePart1.forward, degree1);
            //rotatePart2.Rotate(rotatePart2.forward, degree2);
            //rotatePart2.Rotate(rotatePart3.forward, degree3);
            //rotatePart2.Rotate(rotatePart3.right, degree4);
        }
    }
}
