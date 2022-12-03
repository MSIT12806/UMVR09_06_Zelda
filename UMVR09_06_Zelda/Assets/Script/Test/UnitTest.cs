using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTest : MonoBehaviour
{
    Transform head;
    public Transform testTarget;
    // Start is called before the first frame update
    void Start()
    {

        head = transform.FindAnyChild<Transform>("Character1_Head");
    }
    public Vector3 direction;
    public bool turn;
    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward);
        Debug.DrawLine(transform.position, transform.position + direction, color: new Color(0, 0, 1));
        if (turn)
        {
            Turn(direction);
        }
    }
    private void LateUpdate()
    {

        Look(testTarget);
    }

    public void Turn(Vector3 direction)
    {
        var degree = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        if (degree < 0)
        {
            transform.Rotate(Vector3.up, -1);
        }
        else if (degree > 0)
        {
            transform.Rotate(Vector3.up, 1);
        }
    }

    public void Look(Transform targetHead)
    {
        var degreeY = Vector3.Angle(transform.forward.WithY(), (targetHead.position - head.position).WithY());
        degreeY = degreeY * Mathf.Sign(Vector3.SignedAngle(transform.forward, targetHead.position - head.position, Vector3.up));
        if (degreeY > 80)
        {
            degreeY = 80;
        }
        else if (degreeY < -80)
        {
            degreeY = -80;
        }
        var degreeX = Vector3.Angle((targetHead.position - head.position).WithY(), (targetHead.position - head.position));
        degreeX = degreeX * Mathf.Sign(Vector3.SignedAngle((targetHead.position - head.position).WithY(), targetHead.position - head.position, -transform.right));
        if (degreeX > 40)
        {
            degreeX = 40;
        }
        else if (degreeX < -40)
        {
            degreeX = -40;
        }
        var d = head.forward.WithY();
        d = Quaternion.AngleAxis(degreeY, Vector3.up) * d;
        d = Quaternion.AngleAxis(degreeX, d.GetLocalRight()) * d;
        head.forward = d.normalized;
        direction = d.normalized;
    }
}
