using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraState
{
    public float HorizontalRotateDegree = 0.0f;
    public float VerticalRotateDegree = 0.0f;
    public Vector3 FollowPosition = Vector3.zero;
    public Vector3 CameraDirection = Vector3.zero;
    //public Vector3 RefVel = Vector3.zero;
    public abstract void UpdateCamera();
    public abstract void GetRotateDegree(float sensitivity);
}
public class Default:CameraState
{
    public override void UpdateCamera()
    {
        throw new System.NotImplementedException();
    }

    public override void GetRotateDegree(float sensitivity)
    {
        //Get input in Update
        //Apply changes to physics in FixedUpdate
        float fMX = Input.GetAxis("Mouse X");
        float fMY = Input.GetAxis("Mouse Y");
        HorizontalRotateDegree = fMX * sensitivity;

        VerticalRotateDegree += fMY * sensitivity / 10;
        if (VerticalRotateDegree > 20.0f)
        {
            VerticalRotateDegree = 20.0f;
        }
        else if (VerticalRotateDegree < -45.0f)
        {
            VerticalRotateDegree = -45.0f;
        }
    }
}
public class Stare:CameraState
{
    public override void UpdateCamera()
    {
        throw new System.NotImplementedException();
    }

    public override void GetRotateDegree(float sensitivity)
    {
        //Get input in Update
        //Apply changes to physics in FixedUpdate
        float fMX = Input.GetAxis("Mouse X");
        float fMY = Input.GetAxis("Mouse Y");

        VerticalRotateDegree += fMY * sensitivity / 10;
        if (VerticalRotateDegree > 20.0f)
        {
            VerticalRotateDegree = 20.0f;
        }
        else if (VerticalRotateDegree < -45.0f)
        {
            VerticalRotateDegree = -45.0f;
        }
    }
}
