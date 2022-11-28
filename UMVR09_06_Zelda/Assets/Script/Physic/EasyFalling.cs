using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EasyFalling
{
    public static bool Fall(Transform transform, ref Vector3 initialVelocityPerFrame, int framePerSecond = 30, float EndingYValue = 0)
    {
        float gPerFrame = 9.8f / framePerSecond;
        initialVelocityPerFrame.y -= gPerFrame;

        transform.position += initialVelocityPerFrame;

        if(transform.position.y <= EndingYValue)
        {
            transform.position = transform.position.WithoutY(EndingYValue);
            return false;
        }

        return true;
    }
}
