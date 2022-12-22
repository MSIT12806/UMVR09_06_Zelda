using Ron;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public Transform[] m_StareTarget;
    public float m_StareHeight = 2.5f;
    public float m_LookHeight;
    public float m_LookSmoothTime = 0.1f;
    public float m_FollowSmoothTime = 0.1f;
    public float m_FollowDistance = 5.0f;
    [Range(0.1f, 1000f)]
    public float m_CameraSensitivity = 1.0f;
    public Camera[] cameras;
    public LayerMask avoidLayer;
    public LayerMask transparentLayer;
    public float m_HitMoveDistance = 0.1f;
    private Vector3 m_RefVel = Vector3.zero;
    Camera thisCamera;
    Vector3 lookDirection;
    public int stage;
    CameraState state;
    PicoState gameState;
    public string cameraState { get => state.Name; }
    private float fMX;
    private float fMY;
    bool isNightScene;
    private void Awake()
    {
        var currentScene = SceneManager.GetActiveScene();
        var currentSceneName = currentScene.name;
        if (currentSceneName == "NightScene")
        {
            isNightScene = true;
        }
        else
        {
            isNightScene = false;
        }

        if (isNightScene)
            ObjectManager.myCamera = this;
        else
            ObjectManager2.myCamera = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        state = new Default(m_LookPoint, m_FollowTarget, m_LookHeight, m_FollowDistance);
        state.CameraDirection = m_FollowTarget.forward;
        thisCamera = GetComponent<Camera>();

        var currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "NightScene")
        {
            gameState = ObjectManager.MainCharacter.GetComponent<PicoState>();
        }
        else
        {
            gameState = ObjectManager2.MainCharacter.GetComponent<PicoState>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        RefreshCameraState();
        //GetRotateDegreeByKeyboard();
        GetRotateDegreeByMouse();
        TransparentBlockObject();
        state.SetRotateDegree(fMX, fMY, m_CameraSensitivity);
        state.UpdateParameters(m_LookPoint, m_FollowTarget, m_LookHeight, m_FollowDistance, m_LookSmoothTime);
        //DebugExtension.DebugWireSphere(m_LookPoint.position, 0.5f);
    }

    public void SetDefault()
    {
        state = new Default(m_LookPoint, m_FollowTarget, m_LookHeight, m_FollowDistance);
    }
    public void SetStare()
    {
        state = new Stare(m_LookPoint, m_FollowTarget, m_LookHeight, m_FollowDistance, m_StareTarget[stage]);
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
        if (Input.GetMouseButtonDown(2))
        {
            if (m_StareTarget[stage] != null && m_StareTarget[stage].gameObject.activeSelf)
            {
                if (state.Name == "Default") SetStare();
                else SetDefault();
            }
            else
                SetDefault();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            cameras[(int)gameState.gameState].gameObject.SetActive(!cameras[(int)gameState.gameState].gameObject.activeSelf);
        }
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
        Ray r = new Ray(m_LookPoint.position, -lookDirection);
        if (Physics.SphereCast(r, 0.2f, out RaycastHit rh, state.GetFollowDistance(this.transform), avoidLayer))//形成一個圓柱體？
        {
            Vector3 t = m_LookPoint.position - lookDirection * (rh.distance);// - m_HitMoveDistance
            transform.position = t;
        }
    }

    HashSet<GameObject> transparentObj = new HashSet<GameObject>();
    private void TransparentBlockObject()
    {
        var hitArr = Physics.SphereCastAll(this.transform.position, 0.2f, m_FollowTarget.position.AddY(1.3f) - this.transform.position, Vector3.Distance(this.transform.position, m_FollowTarget.position.AddY(1.3f))-1f, transparentLayer);
        if (hitArr.Length != 0)
        {
            foreach (var hit in hitArr)
            {
                if (hit.transform.gameObject.name != "MainCharacter" && hit.transform.gameObject.name != "Terrain")
                {
                    if (hit.transform.gameObject.tag == "Npc")
                    {
                        var npc = hit.transform.GetComponent<Npc>();
                        var oriSkin = npc.transform.FindAnyChild<Transform>(npc.MaterialAddress);
                        var ditherSkin = npc.transform.FindAnyChild<Transform>(npc.DitherAddress);
                        oriSkin.gameObject.SetActive(false);
                        ditherSkin.gameObject.SetActive(true);
                    }
                    else
                    {
                        SetTransparent(hit.transform.gameObject, Vector3.Distance(this.transform.position, hit.point));
                    }

                    transparentObj.Add(hit.transform.gameObject);
                }
            }
        }
        OnRaycastLeave(hitArr);

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
        if (g.tag == "Npc")
        {
            var npc = g.transform.GetComponent<Npc>();
            var oriSkin = npc.transform.FindAnyChild<Transform>(npc.MaterialAddress);
            var ditherSkin = npc.transform.FindAnyChild<Transform>(npc.DitherAddress);
            oriSkin.gameObject.SetActive(true);
            ditherSkin.gameObject.SetActive(false);
        }
        else
        {
            var renderer = g.GetComponent<Renderer>();
            for (int i = 0; i < g.transform.childCount; i++)
                RecoverTransparentObj(g.transform.GetChild(i).gameObject);

            if (renderer == null)
                return;

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                renderer.materials[i].SetFloat("_MinDistance", 2);
            }
        }

    }
    private void SetTransparent(GameObject g, float distance)
    {
        for (int i = 0; i < g.transform.childCount; i++)
            SetTransparent(g.transform.GetChild(i).gameObject, distance);
        var renderer = g.GetComponent<Renderer>();
        if (renderer == null)
            return;
        for (int i = 0; i < renderer.materials.Length; i++)
        {
            renderer.materials[i].SetFloat("_MinDistance", 0.15f);
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_LookPoint.position, 0.5f);
    }
}
