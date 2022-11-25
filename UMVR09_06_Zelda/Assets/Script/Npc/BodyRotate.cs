using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyRotate : MonoBehaviour
{
    // Start is called before the first frame update
    public float degree = 66.64f;//66.64
    public Transform rotatePart;
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
            rotatePart.Rotate(rotatePart.forward, degree);
    }
}
