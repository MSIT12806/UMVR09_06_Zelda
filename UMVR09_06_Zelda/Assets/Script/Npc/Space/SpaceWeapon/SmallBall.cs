using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBall : MonoBehaviour
{
    // Start is called before the first frame update
    float speedPerSecond = 8.4f;
    readonly float Radius = 1f;
    readonly float Angle = 360f;
    readonly float attackSeconds = 4f;
    float nowAttackSecond;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (nowAttackSecond > 0)
        {
            nowAttackSecond -= Time.deltaTime;
            transform.forward = attackDirection;
            AttackBehavior();
            return;
        }
        AroundBehavior();
    }

    void AroundBehavior()
    {
        attackDirection = (ObjectManager2.MainCharacter.position.WithY() - transform.position.WithY()).normalized;

        var distance = Vector3.Distance(transform.position.WithY(), ObjectManager2.Elena.position.WithY());
        var directionFaceSpace = (ObjectManager2.Elena.position.WithY() - transform.position.WithY()).normalized;
        if (distance >= Radius + 0.1)
        {
            transform.forward = directionFaceSpace;
            transform.position += transform.forward * speedPerSecond * Time.deltaTime;
            return;
        }

        if (distance <= Radius - 0.1)
        {
            transform.forward = -directionFaceSpace;
            transform.position += transform.forward * speedPerSecond * Time.deltaTime;
            return;
        }

        transform.RotateAround(ObjectManager2.Elena.position, ObjectManager2.Elena.up, Time.deltaTime * Angle);


    }
    Vector3 attackDirection;
    void AttackBehavior()
    {

        transform.forward += ((ObjectManager2.MainCharacter.position + ObjectManager2.MainCharacter.forward) - transform.position).normalized;//
        transform.position += transform.forward * speedPerSecond * Time.deltaTime;
        //
        //1.以Space為圓心
        //2.以lico 為半徑
        //3.進行旋轉
        //預計攻擊4秒
    }

    public void Attack()
    {
        nowAttackSecond = attackSeconds;
    }
}
