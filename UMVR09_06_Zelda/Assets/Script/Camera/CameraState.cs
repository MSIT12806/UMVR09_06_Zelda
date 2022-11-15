using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public abstract class CameraState
{
    public float HorizontalRotateDegree = 0.0f;
    public float VerticalRotateDegree = 0.0f;
    public Vector3 FollowPosition = Vector3.zero;
    public Vector3 CameraDirection = Vector3.zero;
    public float FollowDistance = 5.0f;
    public string Name;

    protected Vector3 RefVel = Vector3.zero;
    protected Vector3 lookDirection;
    protected float lookSmoothTime = 0.3f;
    protected float followSmoothTime = 0.3f;
    public abstract void SetRotateDegree(float fMX, float fMY, float sensitivity);
    public abstract void MoveCameraSmoothly(Transform cameraTransform);
    public abstract float GetFollowDistance(Transform cameraTransform);
    public abstract void UpdateParameters(Transform m_LookPoint, Transform m_FollowTarget, float m_LookHeight, float m_FollowDistance, Transform m_StareTarget, float lookSmoothTime);


    /// <summary>
    /// 使用者操縱攝影機視角
    /// </summary>
    public virtual void OperateLookDirection()
    {
        Vector3 targetForwardHorizontalDirection = this.CameraDirection;
        targetForwardHorizontalDirection.y = 0.0f;
        Vector3 unitVectorAfterYRotate = Quaternion.AngleAxis(this.HorizontalRotateDegree, Vector3.up) * targetForwardHorizontalDirection;
        unitVectorAfterYRotate.Normalize();
        Vector3 verticalRotateAxis = Vector3.Cross(Vector3.up, unitVectorAfterYRotate);
        lookDirection = Quaternion.AngleAxis(-this.VerticalRotateDegree, verticalRotateAxis) * unitVectorAfterYRotate;
    }
}
public class Default : CameraState
{
    private Transform _followTarget;
    private float _lookHeight;
    private Transform _lookPoint;


    public Default(Transform lookPoint, Transform followTarget, float lookHeight, float followDistance)
    {
        Name = "Default";
        _lookPoint = lookPoint;
        _lookHeight = lookHeight;
        _followTarget = followTarget;
        FollowDistance = followDistance;
    }

    public override float GetFollowDistance(Transform cameraTransform)
    {
        return Vector3.Distance(_followTarget.position, cameraTransform.position);
    }

    public override void SetRotateDegree(float fMX, float fMY, float sensitivity)
    {
        //Get input in Update
        //Apply changes to physics in FixedUpdate
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

    public override void MoveCameraSmoothly(Transform cameraTransform)
    {        //1. move look point smoothly
        Vector3 vHeadUpPos = _followTarget.position + _lookHeight * Vector3.up;
        _lookPoint.position = Vector3.SmoothDamp(_lookPoint.position, vHeadUpPos, ref RefVel, lookSmoothTime);
        //_lookPoint.position = vHeadUpPos;
        //2. get camera position
        this.FollowPosition = _lookPoint.position - lookDirection * FollowDistance;

        //3. move camera to m_FollowPosition smoothly
        //cameraTransform.position = new Vector3(this.FollowPosition.x, cameraTransform.position.y, this.FollowPosition.z);
        //cameraTransform.position = new Vector3(this.FollowPosition.x, this.FollowPosition.y, this.FollowPosition.z);
        //cameraTransform.position = this.FollowPosition;
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.FollowPosition, followSmoothTime);
    }

    public override void UpdateParameters(Transform lookPoint, Transform followTarget, float lookHeight, float followDistance, Transform m_StareTarget, float lookSmoothTime)
    {
        _lookPoint = lookPoint;
        _lookHeight = lookHeight;
        _followTarget = followTarget;
        FollowDistance = followDistance;
        this.lookSmoothTime = lookSmoothTime;
        Debug.Log(lookSmoothTime);
    }
}
public class Stare : CameraState
{
    private Transform _stareTarget;
    private Transform _followTarget;
    private float _lookHeight;
    private Transform _lookPoint;

    public Stare(Transform lookPoint, Transform followTarget, float lookHeight, float followDistance, Transform stareTarget)
    {
        Name = "Stare";
        _lookPoint = lookPoint;
        _lookHeight = lookHeight;
        _followTarget = followTarget;
        _stareTarget = stareTarget;
        FollowDistance = followDistance;
    }


    public override void SetRotateDegree(float fMX, float fMY, float sensitivity)
    {
        //Get input in Update
        //Apply changes to physics in FixedUpdate

        VerticalRotateDegree += fMY * sensitivity / 10;
        float min = -65.0f;
        float max = 40.0f;
        VerticalRotateDegree = Math.Clamp(VerticalRotateDegree, min, max);
    }


    public override void MoveCameraSmoothly(Transform cameraTransform)
    {
        //1. move look point smoothly
        //Vector3 vHeadUpPos = _stareTarget.position;// + _lookHeight * Vector3.up
        Vector3 vHeadUpPos = _followTarget.position + _lookHeight * Vector3.up;
        _lookPoint.position = Vector3.SmoothDamp(_lookPoint.position, vHeadUpPos, ref RefVel, lookSmoothTime);
        //_lookPoint.position = vHeadUpPos;
        //2. get camera position
        var followTargetWithoutY = _followTarget.position;
        followTargetWithoutY.y = 0;
        var stareTargetWithoutY = _stareTarget.position;
        stareTargetWithoutY.y = 0;

        Vector3 stareTargetToCameraDirection = (followTargetWithoutY - stareTargetWithoutY).normalized;
        FollowPosition = _lookPoint.position + (stareTargetToCameraDirection - lookDirection) * FollowDistance;
        //3. move camera to m_FollowPosition smoothly
        cameraTransform.position = FollowPosition;
    }

    public override float GetFollowDistance(Transform cameraTransform)
    {
        return Vector3.Distance(_followTarget.position, cameraTransform.position);
    }

    public override void UpdateParameters(Transform lookPoint, Transform followTarget, float lookHeight, float followDistance, Transform stareTarget, float lookSmoothTime)
    {
        _lookPoint = lookPoint;
        _lookHeight = lookHeight;
        _followTarget = followTarget;
        _stareTarget = stareTarget;
        FollowDistance = followDistance;
        this.lookSmoothTime = lookSmoothTime;
    }
}
