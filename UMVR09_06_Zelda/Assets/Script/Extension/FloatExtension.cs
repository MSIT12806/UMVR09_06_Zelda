using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtension
{

    public static float AlmostEqual(this float val, float target, float limit)
    {
        if (Math.Abs( val - target )< limit)
        {
            return target;
        }
        else
        {
            return val;
        }
    }
}
