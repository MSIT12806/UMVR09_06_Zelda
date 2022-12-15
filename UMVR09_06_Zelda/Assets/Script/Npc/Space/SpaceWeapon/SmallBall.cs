using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBall : MonoBehaviour
{
    // Start is called before the first frame update
    float speedPerSecond = 8.4f;
    readonly float Radius = 3f;
    readonly float Angle = 160f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Around()
    {
        var distance = Vector3.Distance(transform.position.WithY(), ObjectManager2.Elena.position.WithY());
        var directionFaceSpace = (ObjectManager2.Elena.position.WithY() - transform.position.WithY()).normalized;
        if (distance >= 3.1)
        {
            transform.forward = directionFaceSpace;
            transform.position += transform.forward * speedPerSecond * Time.deltaTime;
            return;
        }

        if (distance <= 2.9)
        {
            transform.forward = -directionFaceSpace;
            transform.position += transform.forward * speedPerSecond * Time.deltaTime;
            return;
        }

        transform.RotateAround(ObjectManager2.Elena.position, ObjectManager2.Elena.up, Time.deltaTime * Angle);

        attackDirection = (ObjectManager2.Elena.position - transform.position).normalized;
    }
    Vector3 attackDirection;
    public void Attack()
    {
        transform.forward = attackDirection + (ObjectManager2.Elena.position + ObjectManager2.Elena.forward);
        transform.position += transform.forward * speedPerSecond * Time.deltaTime;
        //
        //1.�HSpace�����
        //2.�Hlico ���b�|
        //3.�i�����
        //�w�p����4��
    }
}
