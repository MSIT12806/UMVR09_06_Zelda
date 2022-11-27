using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Vector3Extension
{
    public static Vector3 AlmostEqual(this Vector3 val, Vector3 target, float limitDistance)
    {
        if (Vector3.Distance(val, target) < limitDistance)
        {
            return target;
        }
        else
        {
            return val;
        }

    }
    public static Vector3 WithoutY(this Vector3 val)
    {
        var r = val;
        r.y = 0;
        return r;
    }
}
