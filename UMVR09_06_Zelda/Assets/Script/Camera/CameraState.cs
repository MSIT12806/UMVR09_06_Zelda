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
    protected Vector3 lookDirection;
    public abstract void GetRotateDegree(float sensitivity);
    /// <summary>
    /// 使用者操縱攝影機視角
    /// </summary>
    public virtual void OperateLookDirection()
    {
        Vector3 targetForwardHorizontalVector = this.CameraDirection;
        targetForwardHorizontalVector.y = 0.0f;
        Vector3 unitVectorAfterYRotate = Quaternion.AngleAxis(this.HorizontalRotateDegree, Vector3.up) * targetForwardHorizontalVector;
        unitVectorAfterYRotate.Normalize();
        Vector3 verticalRotateAxis = Vector3.Cross(Vector3.up, unitVectorAfterYRotate);
        lookDirection = Quaternion.AngleAxis(-this.VerticalRotateDegree, verticalRotateAxis) * unitVectorAfterYRotate;
    }
    public abstract void MoveCameraSmoothly(Transform cameraTransform);

}
public class Default : CameraState
{
    private Transform _followTarget;
    private float _lookHeight;
    private Transform _lookPoint;

    private float lookSmoothTime = 0.1f;
    private float followDistance = 5.0f;
    private float followSmoothTime = 0.1f;

    Vector3 m_RefVel = Vector3.zero;
    public Default(Transform lookPoint, Transform followTarget, float lookHeight)
    {
        _lookPoint = lookPoint;
        _lookHeight = lookHeight;
        _followTarget = followTarget;
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

    public override void MoveCameraSmoothly(Transform cameraTransform)
    {        //1. move look point smoothly
        Vector3 vHeadUpPos = _followTarget.position + _lookHeight * Vector3.up;
        // m_LookPoint.position = Vector3.Lerp(m_LookPoint.position, vHeadUpPos, m_LookSmoothTime);
        _lookPoint.position = Vector3.SmoothDamp(_lookPoint.position, vHeadUpPos, ref m_RefVel, lookSmoothTime);
        //2. get camera position
        this.FollowPosition = _lookPoint.position - lookDirection * followDistance;

        //3. move camera to m_FollowPosition smoothly
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.FollowPosition, followSmoothTime);
    }


}
public class Stare : CameraState
{
    /*
     * 1. 離怪太近會卡住
     * 2. 調整上下
     */
    private Transform _stareTarget;
    private Transform _followTarget;
    private float _lookHeight;
    private Transform _lookPoint;
    private float lookSmoothTime = 0.1f;
    private float followDistance = 5.0f;
    private float followSmoothTime = 0.1f;

    Vector3 m_RefVel = Vector3.zero;
    float m_LookSmoothTime = 0.1f;

    public Stare(Transform lookPoint, Transform followTarget, float lookHeight, Transform stareTarget)
    {
        _lookPoint = lookPoint;
        _lookHeight = lookHeight;
        _followTarget = followTarget;
        _stareTarget = stareTarget;
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


    public override void MoveCameraSmoothly(Transform cameraTransform)
    {
        //1. move look point smoothly
        Vector3 vHeadUpPos = _stareTarget.position + _lookHeight * Vector3.up;
        _lookPoint.position = Vector3.SmoothDamp(_lookPoint.position, vHeadUpPos, ref m_RefVel, m_LookSmoothTime);
        //2. get camera position
        Vector3 cameraDirection = _stareTarget.position - _followTarget.position;
        var vToGetCameraPosition = -cameraDirection.normalized;
        FollowPosition = _followTarget.position + vToGetCameraPosition * followDistance;
        //3. move camera to m_FollowPosition smoothly
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, FollowPosition, followSmoothTime);
    }

}
