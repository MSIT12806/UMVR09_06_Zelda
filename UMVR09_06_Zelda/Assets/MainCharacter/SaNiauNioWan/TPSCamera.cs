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
    public float m_LookHeight;
    public float m_LookSmoothTime = 0.1f;
    public float m_FollowSmoothTime = 0.1f;
    public float m_FollowDistance = 5.0f;
    [Range(0.1f, 1000f)]
    public float m_CameraSensitivity = 1.0f;

    public float m_FollowHeight = 0.0f;
    public LayerMask m_HitLayers;
    public float m_HitMoveDistance = 0.1f;
    private float horizontalRotateDegreeByMouseMoving = 0.0f;
    private float verticalRotateDegreeByMouseMoving = 0.0f;
    private Vector3 m_FollowPosition = Vector3.zero;
    private Vector3 cameraDirection = Vector3.zero;
    private Vector3 m_RefVel = Vector3.zero;

    Vector3 lookDirection;
    // Start is called before the first frame update
    void Start()
    {
        cameraDirection = m_FollowTarget.forward;
    }

    // Update is called once per frame
    void Update()
    {
        //TransparentBlockObject();
        GetRotateDegreeByMouse();
    }

    private void LateUpdate()
    {
        lookDirection = cameraDirection; //default 
        ChangeLookDirection();
        MoveCameraSmoothly();

        this.transform.LookAt(m_LookPoint);
        AdjustPositionToAvoidObstruct(this.transform.forward);

        this.transform.LookAt(m_LookPoint);

        RefreshCameraDirectionValue();
    }

    #region private methods

    private void GetRotateDegreeByMouse()
    {
        //Get input in Update
        //Apply changes to physics in FixedUpdate
        float fMX = Input.GetAxis("Mouse X");
        float fMY = Input.GetAxis("Mouse Y");
        horizontalRotateDegreeByMouseMoving = fMX * m_CameraSensitivity;

        verticalRotateDegreeByMouseMoving += fMY * m_CameraSensitivity / 10;
        if (verticalRotateDegreeByMouseMoving > 20.0f)
        {
            verticalRotateDegreeByMouseMoving = 20.0f;
        }
        else if (verticalRotateDegreeByMouseMoving < -45.0f)
        {
            verticalRotateDegreeByMouseMoving = -45.0f;
        }
    }
    private void GetRotateDegreeByKeyboard()
    {
        float fMX = Input.GetAxis("CameraHor");
        float fMY = Input.GetAxis("CameraVer");
        horizontalRotateDegreeByMouseMoving = fMX * m_CameraSensitivity;

        verticalRotateDegreeByMouseMoving += fMY * m_CameraSensitivity / 10;
        if (verticalRotateDegreeByMouseMoving > 20.0f)
        {
            verticalRotateDegreeByMouseMoving = 20.0f;
        }
        else if (verticalRotateDegreeByMouseMoving < -45.0f)
        {
            verticalRotateDegreeByMouseMoving = -45.0f;
        }
    }
    private void RefreshCameraDirectionValue()
    {
        cameraDirection = transform.forward;
    }

    private void MoveCameraSmoothly()
    {
        //1. move look point smoothly
        Vector3 vHeadUpPos = m_FollowTarget.position + m_LookHeight * Vector3.up;
        // m_LookPoint.position = Vector3.Lerp(m_LookPoint.position, vHeadUpPos, m_LookSmoothTime);
        m_LookPoint.position = Vector3.SmoothDamp(m_LookPoint.position, vHeadUpPos, ref m_RefVel, m_LookSmoothTime);
        //2. get camera position
        m_FollowPosition = m_LookPoint.position - lookDirection * m_FollowDistance;

        //3. move camera to m_FollowPosition smoothly
        transform.position = Vector3.Lerp(transform.position, m_FollowPosition, m_FollowSmoothTime);
    }

    private void ChangeLookDirection()
    {
        Vector3 targetForwardHorizontalVector = cameraDirection;
        targetForwardHorizontalVector.y = 0.0f;
        Vector3 unitVectorAfterYRotate = Quaternion.AngleAxis(horizontalRotateDegreeByMouseMoving, Vector3.up) * targetForwardHorizontalVector;
        unitVectorAfterYRotate.Normalize();
        Vector3 verticalRotateAxis = Vector3.Cross(Vector3.up, unitVectorAfterYRotate);
        lookDirection = Quaternion.AngleAxis(-verticalRotateDegreeByMouseMoving, verticalRotateAxis) * unitVectorAfterYRotate;
    }
    private void AdjustPositionToAvoidObstruct(Vector3 lookDirection)
    {
        Ray r = new Ray(m_LookPoint.position, -lookDirection);
        // first method.
        //if(Physics.Raycast(r, out rh, m_FollowDistance, m_HitLayers))
        //{
        //    Vector3 t = rh.point + finialVec* m_HitMoveDistance;
        //    transform.position = t;
        //}
        if (Physics.SphereCast(r, 0.5f, out RaycastHit rh, m_FollowDistance, m_HitLayers))
        {
            Debug.Log(rh.transform.gameObject.name);
            Vector3 t = m_LookPoint.position - lookDirection * (rh.distance - m_HitMoveDistance);
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(m_LookPoint.position, 0.5f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_FollowPosition, 0.5f);
    }
}
