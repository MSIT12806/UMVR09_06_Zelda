using Ron;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(Collider))]
public class Npc : MonoBehaviour
{
    // Start is called before the first frame update

    /*
     * 群體運動？     
     */

    /*
     * 1. 對 collider 的碰撞偵測與碰撞反應。  記得問老師怎麼處理 npc碰撞 (raycast? 兩兩算距離?)
     * 1.1 幫每個物件設定半徑，畫raycast
     * 1.2 輪巡(容器？)幫每個物件算距離
     */
    [SerializeField] LayerMask layerMask;
    [SerializeField] LayerMask terrainLayer;
    Collider collider;
    Animator animator;
    PicoState picoState;
    public Vector3 nextPosition;
    public GameState gameState;
    public bool collide { get; set; }
    public bool collideFront { get; set; }
    public bool Alive { get => Hp > 0; }
    public bool OnGround;
    NpcHelper stateManager;
    List<Material> materials;
    Color oriColor = new Color(209, 209, 209);
    float oriPower = 0.254f;
    float oriMask = 0.199f;
    bool oriEnabled = false;
    IKController ik;
    [HideInInspector] public float MaxHp;
    public float Hp;
    public string MaterialAddress;
    Transform renderer;
    private void Awake()
    {
        nextPosition = Vector3.zero;
        animator = GetComponent<Animator>();
        picoState = GetComponent<PicoState>();
        MaxHp = Hp;
        ik = GetComponent<IKController>();
        materials = new List<Material>();
        if (string.IsNullOrEmpty(MaterialAddress) == false)
        {
            renderer = transform.FindAnyChild<Transform>(MaterialAddress);
            var r = renderer.GetComponent<Renderer>();
            materials.Add(r.materials[0]);
            if (r.materials.Length >= 4)
            {
                materials.Add(r.materials[2]);
                materials.Add(r.materials[3]);
            }
            oriColor = materials[0].GetColor("_RimLightColor");
            oriPower = materials[0].GetFloat("_RimLight_Power");
            oriMask = materials[0].GetFloat("_RimLight_InsideMask");

            oriEnabled = renderer.gameObject.activeSelf;
            //  print(c.name);
            //material = b.FirstOrDefault(i => i.name == "Mt_usao_Main");
        }
    }
    void Start()
    {
        stateManager = ObjectManager.StateManagers[this.gameObject.GetInstanceID()];
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf == false) return;

        OnGround = StandOnTerrain();
        collide = StaticCollision();
        TimePause();
        if (stopAnimationMoving > 0)
        {
            stopAnimationMoving--;
            var hitInfos = Physics.OverlapSphere(transform.position + new Vector3(0, 0.7f, 0), stateManager.Radius, layerMask);
            if (hitInfos.Count() > 0)
            {

                animator.applyRootMotion = false;
            }
        }
    }
    bool pause;
    float beforePauseAnimatorSpeed;
    Vector3 beforePauseInitVelocity;
    Vector3 beforePauseNextPosition;
    void TimePause()
    {
        if (pauseTime > 0)
        {
            pause = true;
            pauseTime -= Time.deltaTime;
            beforePauseInitVelocity += initVel;
            initVel = Vector3.zero;
            beforePauseNextPosition += nextPosition;
            nextPosition = Vector3.zero;
        }
        if (animator.speed == 0 && pauseTime <= 0)
        {
            pause = false;
            initVel = beforePauseInitVelocity;
            beforePauseInitVelocity = Vector3.zero;
            nextPosition = beforePauseNextPosition;
            beforePauseNextPosition = Vector3.zero;
            animator.speed = beforePauseAnimatorSpeed;
            if (ik != null)
            {

                ik.enabled = true;
            }
            renderer.gameObject.SetActive(oriEnabled);
            if (materials != null)
            {
                foreach (var item in materials)
                {
                    item.SetColor("_RimLightColor", oriColor);
                    item.SetFloat("_RimLight_Power", oriPower);
                    item.SetFloat("_RimLight_InsideMask", oriMask);
                }
            }
        }
    }
    private void LateUpdate()
    {

        if (!collide)
        {
            NpcCollision();
            LerpToNextPosition();
        }
        FreeFall();
    }
    float lerpTime;
    private void LerpToNextPosition()
    {
        if (lerpTime < Time.time)
        {
            nextPosition = Vector3.zero;
            lerpTime = Time.time + 3;
            return;
        }
        if (nextPosition != Vector3.zero)
        {
            if (transform.position.WithY().AlmostEqual(nextPosition, 0.03f) == nextPosition.WithY())
            {
                transform.position = nextPosition;
                nextPosition = Vector3.zero;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, nextPosition, 0.1f).AlmostEqual(nextPosition, 0.2f);
            }

        }
    }

    public DamageData Attack()
    {
        //請善用狀態機處理攻擊判定
        return DamageData.NoDamage;
    }
    float pauseTime;
    public void GetHurt(DamageData damageData)
    {
        //print("有被打喔");
        stateManager.GetHurt(damageData);
        //效果處裡

        if (damageData.DamageState == null) return;

        if (damageData.DamageState.damageState == DamageState.TimePause)
        {
            pauseTime = damageData.DamageState.KeepTime;
            beforePauseAnimatorSpeed = animator.speed;
            animator.speed = 0;
            beforePauseInitVelocity = initVel;
            initVel = Vector3.zero;
            beforePauseNextPosition = nextPosition;
            nextPosition = Vector3.zero;
            if (ik != null)
            {
                ik.enabled = false;
            }
            if (materials != null)
            {
                print("LIGHT");
                foreach (var item in materials)
                {
                    item.SetColor("_RimLightColor", new Color(255, 255, 0));
                    item.SetFloat("_RimLight_Power", 1);
                    item.SetFloat("_RimLight_InsightMask", 0.0001f);
                }
                renderer.gameObject.SetActive(true);
            }
        }
    }
    public void PlayAnimation(string aniName)
    {
        if (pause) return;
        animator.Play(aniName);
    }

    bool StaticCollision(float radius = 0.23f, float maxDistance = 0.3f)
    {
        collideFront = false;
        animator.applyRootMotion = true;
        var hitSomethingWhenMoving = Physics.SphereCast(this.transform.position + new Vector3(0, 0.7f, 0), radius, transform.forward, out var hitInfo, 0.5f, layerMask);
        var hitInfos = Physics.OverlapSphere(transform.position + new Vector3(0, 0.7f, 0), stateManager.Radius, layerMask);
        var hitSomething = hitSomethingWhenMoving || hitInfos.Count() > 0;
        if (hitSomething && hitInfo.transform != this.transform)
        {
            if (hitSomethingWhenMoving && this.name != "MainCharacter") //讓 npc 隨機旋轉，離開障礙物
            {
                animator.applyRootMotion = false;

                var rotateWay = Vector3.SignedAngle(transform.forward, hitInfo.point - transform.position, Vector3.up);
                transform.Rotate(0, -Mathf.Sign(rotateWay) * 2, 0);
            }
            nextPosition = Vector3.zero;//取消程式位移
            collideFront = hitSomethingWhenMoving;
            if (hitInfos.Count() > 0)
            {
                var closestPoint = hitInfos[0].ClosestPoint(transform.position);
                transform.position -= (closestPoint - transform.position).normalized * 0.1f;
            }
            return hitSomethingWhenMoving || hitSomething;//回報碰撞，取消美術位移
        }
        return false;
    }

    void NpcCollision()
    {
        if (collide) return;
        if (pause) return;
        if (ObjectManager.NpcsAlive == null) return;
        foreach (var item in ObjectManager.NpcsAlive.Values)
        {
            if (item == this) continue;

            var nh = ObjectManager.StateManagers[item.gameObject.GetInstanceID()];

            var distance = Vector3.Distance(this.transform.position, item.transform.position);
            if (distance > nh.Radius + stateManager.Radius) continue;
            if (name == "MainCharacter" && item.name == "Blue Variant")
            {
                print("aaa");
                print(nh.Radius + stateManager.Radius);
            }
            var direction = (item.transform.position - this.transform.position).normalized;
            this.transform.position -= direction * stateManager.CollisionDisplacement;
        }
    }
    bool StandOnTerrain()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + (Vector3.up * 1f), Vector3.down, out hitInfo, 5f, terrainLayer))
        {
            //Debug.Log(hitInfo.transform.name);
            //m_GroundNormal = hitInfo.normal;

            // SetMoveParas(true, true);
            if (hitInfo.point.y - transform.position.y < 0f)
            {
                //transform.Translate(0f, 0.01f,0f);
                Vector3 vec = transform.position;
                vec.y = hitInfo.point.y;
                transform.position = vec;
            }
            else if (hitInfo.point.y - transform.position.y > 0f)
            {
                Vector3 vec = transform.position;
                vec.y = hitInfo.point.y;
                transform.position = vec;
            }

            return true;
        }
        else
        {
            //SetMoveParas(false, true);
            return false;
        }
    }
    public Vector3 initVel;
    float terrainHeight = float.MinValue;
    public bool grounded;
    void FreeFall()
    {
        terrainHeight = TerrainY();
        if (!grounded)
        {
            if (collide)
            {
                print("touch");
                initVel.x = 0;
                initVel.z = 0;
            }
            grounded = !EasyFalling.Fall(transform, ref initVel, EndingYValue: terrainHeight);
            nextPosition = Vector3.zero;
        }

        SetAnimationGroundedParameter();
    }

    private void SetAnimationGroundedParameter()
    {
        if (grounded && !animator.GetBool("Grounded"))
        {
            animator.SetBool("Grounded", UnityEngine.Random.value * UnityEngine.Random.value > 0.8f);
        }
    }

    float TerrainY()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + (Vector3.up * 1f), Vector3.down, out hitInfo, 5f, terrainLayer))
        {
            return hitInfo.point.y;
        }

        return float.MinValue;
    }

    public void KnockOff(Vector3 force)
    {

        animator.SetBool("Grounded", false);
        grounded = false;
        initVel = force;
    }
    int stopAnimationMoving;
    public void CancelMotionIfCollided(int keepFrame)
    {
        stopAnimationMoving = keepFrame;
    }

    public void Die()
    {
        stateManager.Die();
    }
    //private void OnDrawGizmos()
    //{
    //    //檢查球射線是否有交點
    //    float sphereCastRadius = 0.23f;
    //    float range = 0.3f;
    //    var position = this.transform.position + new Vector3(0, 0.7f, 0);
    //    var direction = transform.forward;
    //    Gizmos.DrawWireSphere(position, sphereCastRadius);

    //    RaycastHit hit;
    //    if (Physics.SphereCast(position, sphereCastRadius, direction, out hit, range, layerMask) && hit.transform.gameObject.layer != this.gameObject.layer)
    //    {
    //        Gizmos.color = Color.green;
    //        Gizmos.DrawSphere(hit.point, 0.1f);
    //        //Debug.DrawLine(transform.position, sphereCastMidpoint, Color.green);
    //    }
    //    else
    //    {
    //        Gizmos.color = Color.red;
    //        Vector3 sphereCastMidpoint = position - direction * sphereCastRadius + direction * range;// + (direction * (range - sphereCastRadius));
    //        Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
    //        //Debug.DrawLine(transform.position, sphereCastMidpoint, Color.red);
    //    }
    //}
}
