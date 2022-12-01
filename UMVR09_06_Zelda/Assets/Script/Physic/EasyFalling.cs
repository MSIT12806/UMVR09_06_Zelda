using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EasyFalling
{
    public static bool Fall(Transform transform, ref Vector3 initialVelocityPerFrame, int framePerSecond = 30, float EndingYValue = 0)
    {
        Debug.Log(initialVelocityPerFrame);
        //float gPerFrame = 9.8f / framePerSecond;
        float gPerFrame = 1f / framePerSecond;
        initialVelocityPerFrame.y -= gPerFrame;

        transform.position += initialVelocityPerFrame;
        if (transform.position.y <= EndingYValue)//避免掉落到超過 terrain
        {
            initialVelocityPerFrame = Vector3.zero;
            Debug.Log(initialVelocityPerFrame);
            transform.position = transform.position.WithY(EndingYValue);
            return false;
        }
        return true;
    }
}
