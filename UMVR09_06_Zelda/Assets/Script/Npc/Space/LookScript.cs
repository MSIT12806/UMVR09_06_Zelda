using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyLookAt
{
    public static void Look(Transform target , Transform selfTransform , float perFrameDegree)
    {
        //if (FreezeTime > 0) return;
        Vector3 dir = target.position - selfTransform.position;
        dir.y = 0;
        dir.Normalize();
        float dot = Vector3.Dot(selfTransform.forward, dir);

        if (dot > 1) dot = 1;
        else if (dot < -1) dot = -1;

        float radian = Mathf.Acos(dot);
        float degree = radian * Mathf.Rad2Deg;

        Vector3 vCross = Vector3.Cross(selfTransform.forward, dir);
        if (degree > perFrameDegree)
        {
            if (vCross.y < 0)
                selfTransform.Rotate(0, -perFrameDegree, 0);
            else
                selfTransform.Rotate(0, perFrameDegree, 0);
        }
        else
        {
            if (vCross.y < 0)
                selfTransform.Rotate(0, -degree, 0);
            else
                selfTransform.Rotate(0, degree, 0);
        }
    }
}

