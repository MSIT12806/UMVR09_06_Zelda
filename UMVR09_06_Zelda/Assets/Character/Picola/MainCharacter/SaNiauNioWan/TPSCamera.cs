using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    //public float m_FollowHeight = 0.0f;
    public LayerMask m_HitLayers;
    public float m_HitMoveDistance = 0.1f;
    //private float horizontalRotateDegree = 0.0f;
    //private float verticalRotateDegree = 0.0f;
    //private Vector3 m_FollowPosition = Vector3.zero;
    //private Vector3 cameraDirection = Vector3.zero;
    private Vector3 m_RefVel = Vector3.zero;

    Vector3 lookDirection;

    CameraState state;
    private float fMX;
    private float fMY;

    // Start is called before the first frame update
    void Start()
    {
        state = new Default(m_LookPoint, m_FollowTarget, m_LookHeight, m_FollowDistance);
        state.CameraDirection = m_FollowTarget.forward;
    }

    // Update is called once per frame
    void Update()
    {
        RefreshCameraState();
        //GetRotateDegreeByKeyboard();
        GetRotateDegreeByMouse();
        TransparentBlockObject();
        state.SetRotateDegree(fMX, fMY, m_CameraSensitivity);
        state.UpdateParameters(m_LookPoint, m_FollowTarget, m_LookHeight, m_FollowDistance, m_StareTarget, m_LookSmoothTime);
        //DebugExtension.DebugWireSphere(m_LookPoint.position, 0.5f);
    }
    private void LateUpdate()
    {
        state.OperateLookDirection();
        state.MoveCameraSmoothly(this.transform);

        this.transform.LookAt(m_LookPoint);
        AdjustPositionToAvoidObstruct(this.transform.forward);
        this.transform.LookAt(m_LookPoint);
        //this.transform.LookAt(m_FollowTarget);


        RefreshCameraDirectionValue();
    }
    private void RefreshCameraState()
    {
        if (Input.GetMouseButtonDown(2))//defaut camera
            state = state.Name=="Default" ? new Stare(m_LookPoint, m_FollowTarget, m_LookHeight, m_FollowDistance, m_StareTarget): new Default(m_LookPoint, m_FollowTarget, m_LookHeight, m_FollowDistance);
        //if (Input.GetKey(KeyCode.Alpha1))//stare camera
        //{
        //    state = new Stare(m_LookPoint, m_FollowTarget, m_LookHeight, m_FollowDistance, m_StareTarget);
        //    state.VerticalRotateDegree -= 20f;
        //}
    }

    #region private methods


    private void GetRotateDegreeByKeyboard()
    {
        fMX = Input.GetAxis("CameraHor");
        fMY = Input.GetAxis("CameraVer");
    }
    private void GetRotateDegreeByMouse()
    {
        //Get input in Update
        //Apply changes to physics in FixedUpdate
        fMX = Input.GetAxis("Mouse X");
        fMY = Input.GetAxis("Mouse Y");
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

    HashSet<GameObject> transparentObj = new HashSet<GameObject>();
    Dictionary<int, (Shader, Color)> materialShader = new Dictionary<int, (Shader, Color)>();
    private void TransparentBlockObject()
    {
        var hitArr = Physics.RaycastAll(this.transform.position, m_FollowTarget.position - this.transform.position, Vector3.Distance(this.transform.position, m_FollowTarget.position));
        if (hitArr.Length != 0)
        {
            foreach (var hit in hitArr)
            {
                if (hit.transform.gameObject.name != "MainCharacter" && hit.transform.gameObject.name != "Terrain")
                {
                    SetTransparent(hit.transform.gameObject);
                    transparentObj.Add(hit.transform.gameObject);
                }
            }
            OnRaycastLeave(hitArr);
        }

    }

    private void OnRaycastLeave(IEnumerable<RaycastHit> hitArr)
    {
        var item = transparentObj.FirstOrDefault(go => !hitArr.Any(i => i.transform.gameObject == go));
        if (item == null) return;
        RecoverTransparentObj(item.transform.gameObject);
        transparentObj.Remove(item.transform.gameObject);
    }
    private void RecoverTransparentObj(GameObject g)
    {
        var renderer = g.GetComponent<Renderer>();
        for (int i = 0; i < g.transform.childCount; i++)
            RecoverTransparentObj(g.transform.GetChild(i).gameObject);

        if (renderer == null)
        {
            return;
        }

        for (int i = 0; i < renderer.materials.Length; i++)
        {
            materialShader.TryGetValue(renderer.materials[i].GetHashCode(), out var tuple);
            renderer.materials[i].shader = tuple.Item1;
            renderer.materials[i].SetColor("_Color", tuple.Item2);
            materialShader.Remove(renderer.materials[i].GetHashCode());
            Debug.Log(g.name + " is remove in materialShader");
        }
    }
    private void SetTransparent(GameObject g)
    {
        for (int i = 0; i < g.transform.childCount; i++)
            SetTransparent(g.transform.GetChild(i).gameObject);
        var renderer = g.GetComponent<Renderer>();
        if (renderer == null)
        {
            return;
        }
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            if (materialShader.ContainsKey(renderer.materials[i].GetHashCode()))
            {
                break;
            }

            materialShader.Add(renderer.materials[i].GetHashCode(), (renderer.materials[i].shader, renderer.materials[i].GetColor("_Color")));
            renderer.materials[i].shader = Shader.Find("Transparent/Diffuse");
            renderer.materials[i].SetColor("_Color", new Color(1, 1, 1, 0.1f));
            Debug.Log(g.name + " is add in materialShader");
        }
    }
    #endregion

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(m_LookPoint.position, 0.5f);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(state.FollowPosition, 0.5f);
    //}
}
