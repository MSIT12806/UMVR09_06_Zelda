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
    public static Vector3 WithY(this Vector3 val, float yVal = 0)
    {
        var r = val;
        r.y = yVal;
        return r;
    }
    public static Vector3 WithX(this Vector3 val, float xVal = 0)
    {
        var r = val;
        r.x = xVal;
        return r;
    }

    public static Vector3 AddY(this Vector3 val, float addVal)
    {
        val.y += addVal;
        return val;
    }

    public static Vector3 GetLocalRight(this Vector3 forward)
    {
        return Vector3.Cross(forward.normalized, Vector3.up);
    }
    public static Vector3 GetLocalUp(this Vector3 forward)
    {
        return Vector3.Cross(forward.normalized, Vector3.right);
    }
}
