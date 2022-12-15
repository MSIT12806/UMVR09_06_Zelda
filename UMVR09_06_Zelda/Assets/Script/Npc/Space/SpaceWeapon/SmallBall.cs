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
        Around();
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
    }

    public void Attack()
    {
     //以二者中間點為圓心，Lico位置為半徑旋轉
     //1.選定圓心
     //2.計算半徑
     //3.進行旋轉
     //預計攻擊4秒
    }
}
