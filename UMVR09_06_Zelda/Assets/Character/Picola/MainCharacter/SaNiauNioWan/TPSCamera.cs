using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 此元件會同步於一個會平滑跟隨的點(m_LookPoint)，並看向m_FollowTarget。
/// </summary>
public class TPSCamera : MonoBehaviour
{

    /*
     重構思路：
    讓攝影機可以設置於 m_LookPoint 四周，設定效果。
     */
    public Transform m_LookPoint;
    public Transform m_FollowTarget;
    public Transform m_StareTarget;
    public float m_StareHeight = 2.5f;
    public float m_LookHeight;
    public float m_LookSmoothTime = 0.1f;
    public float m_FollowSmoothTime = 0.1f;
    public float m_FollowDistance = 5.0f;
    [Range(0.1f, 1000f)]
    public float m_CameraSensitivity = 1.0f;

    public float m_FollowHeight = 0.0f;
    public LayerMask m_HitLayers;
    public float m_HitMoveDistance = 0.1f;
    //private float horizontalRotateDegree = 0.0f;
    //private float verticalRotateDegree = 0.0f;
    //private Vector3 m_FollowPosition = Vector3.zero;
    //private Vector3 cameraDirection = Vector3.zero;
    private Vector3 m_RefVel = Vector3.zero;

    Vector3 lookDirection;

    CameraState state;

    // Start is called before the first frame update
    void Start()
    {
        state = new Default(m_LookPoint, m_FollowTarget, m_LookHeight, m_FollowDistance);
        state.CameraDirection = m_FollowTarget.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha0))//defaut camera
            state = new Default(m_LookPoint, m_FollowTarget, m_LookHeight, m_FollowDistance);
        if (Input.GetKey(KeyCode.Alpha1))//stare camera
        {
            state = new Stare(m_LookPoint, m_FollowTarget, m_LookHeight, m_StareTarget);
            state.VerticalRotateDegree -= 20f;
        }
        //TransparentBlockObject();
        state.GetRotateDegree(m_CameraSensitivity);
    }

    private void LateUpdate()
    {
        lookDirection = state.CameraDirection; //default 
        state.OperateLookDirection();
        state.MoveCameraSmoothly(this.transform);

        this.transform.LookAt(m_LookPoint);
        AdjustPositionToAvoidObstruct(this.transform.forward);

        this.transform.LookAt(m_LookPoint);

        RefreshCameraDirectionValue();
    }

    #region private methods


    private void GetRotateDegreeByKeyboard()
    {
        float fMX = Input.GetAxis("CameraHor");
        float fMY = Input.GetAxis("CameraVer");
        state.HorizontalRotateDegree = fMX * m_CameraSensitivity;

        state.VerticalRotateDegree += fMY * m_CameraSensitivity / 10;
        if (state.VerticalRotateDegree > 20.0f)
        {
            state.VerticalRotateDegree = 20.0f;
        }
        else if (state.VerticalRotateDegree < -45.0f)
        {
            state.VerticalRotateDegree = -45.0f;
        }
    }
    private void RefreshCameraDirectionValue()
    {
        state.CameraDirection = transform.forward;
    }

    private void MoveCameraSmoothly()
    {
        //1. move look point smoothly
        Vector3 vHeadUpPos = m_FollowTarget.position + m_LookHeight * Vector3.up;
        // m_LookPoint.position = Vector3.Lerp(m_LookPoint.position, vHeadUpPos, m_LookSmoothTime);
        m_LookPoint.position = Vector3.SmoothDamp(m_LookPoint.position, vHeadUpPos, ref m_RefVel, m_LookSmoothTime);
        //2. get camera position
        state.FollowPosition = m_LookPoint.position - lookDirection * m_FollowDistance;

        //3. move camera to m_FollowPosition smoothly
        transform.position = Vector3.Lerp(transform.position, state.FollowPosition, m_FollowSmoothTime);
    }

    private void ChangeLookDirection()
    {
        Vector3 targetForwardHorizontalVector = state.CameraDirection;
        targetForwardHorizontalVector.y = 0.0f;
        Vector3 unitVectorAfterYRotate = Quaternion.AngleAxis(state.HorizontalRotateDegree, Vector3.up) * targetForwardHorizontalVector;
        unitVectorAfterYRotate.Normalize();
        Vector3 verticalRotateAxis = Vector3.Cross(Vector3.up, unitVectorAfterYRotate);
        lookDirection = Quaternion.AngleAxis(-state.VerticalRotateDegree, verticalRotateAxis) * unitVectorAfterYRotate;
    }
    private void AdjustPositionToAvoidObstruct(Vector3 lookDirection)
    {
        //lookDirection = this.transform.position - m_LookPoint.position;
        Ray r = new Ray(m_LookPoint.position, -lookDirection);
        // first method.
        //if(Physics.Raycast(r, out rh, m_FollowDistance, m_HitLayers))
        //{
        //    Vector3 t = rh.point + finialVec* m_HitMoveDistance;
        //    transform.position = t;
        //}

        if (Physics.SphereCast(r, 0.5f, out RaycastHit rh, state.GetFollowDistance(this.transform), m_HitLayers))//形成一個圓柱體？
        {
            Vector3 t = m_LookPoint.position - lookDirection * (rh.distance);// - m_HitMoveDistance
            transform.position = t;
        }
    }

    private void TransparentBlockObject()
    {
        if (Physics.Raycast(this.transform.position, m_FollowTarget.position - this.transform.position, out var hit, Vector3.Distance(this.transform.position, m_FollowTarget.position)))
        {
            var trees = Terrain.activeTerrain.terrainData.treeInstances;
            if (hit.transform.gameObject.name != "MainCharacter")
            {
                Debug.Log(hit.transform.gameObject.name);//Terrain
                SetTransparent(hit.transform.gameObject);
            }
        }

    }
    private void SetTransparent(GameObject g)
    {
        for (int i = 0; i < g.GetComponent<Renderer>().materials.Length; i++)
        {
            g.GetComponent<Renderer>().materials[i].shader = Shader.Find("Transparent/Diffuse");
            g.GetComponent<Renderer>().materials[i].SetColor("_Color", new Color(1, 1, 1, 0.1f));
        }
        for (int i = 0; i < g.transform.childCount; i++)
            SetTransparent(g.transform.GetChild(i).gameObject);
    }
    #endregion

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(m_LookPoint.position, 0.5f);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(state.FollowPosition, 0.5f);
    //}
}
