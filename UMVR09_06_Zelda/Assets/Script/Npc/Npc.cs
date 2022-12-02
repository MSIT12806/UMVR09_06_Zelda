using System;
using System.Collections;
using System.Collections.Generic;
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
    public GameState gameState { get => (GameState)picoState.gameState; }
    public bool collide { get; set; }
    public bool Alive { get => Hp > 0;  }
    public bool OnGround;
    NpcHelper stateManager;
    public float Hp;
    void Start()
    {
        nextPosition = Vector3.zero;
        stateManager = ObjectManager.StateManagers[this.gameObject.GetInstanceID()];
        animator = GetComponent<Animator>();
        picoState = GetComponent<PicoState>();
    }

    // Update is called once per frame
    void Update()
    {
        OnGround = StandOnTerrain();
        collide = StaticCollision();
        NpcCollision();
        LerpToNextPosition();
        FreeFall();
    }
    private void LerpToNextPosition()
    {
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
    public void GetHurt(DamageData damageData)
    {
        stateManager.GetHurt(damageData);
    }


    bool StaticCollision(float radius = 0.23f, float maxDistance = 0.3f)
    {
        var hitSomethingWhenMoving = Physics.SphereCast(this.transform.position + new Vector3(0, 0.7f, 0), radius, transform.forward, out var hitInfo, maxDistance, layerMask);

        var hitSomething = hitSomethingWhenMoving || Physics.SphereCast(this.transform.position + new Vector3(0, 0.7f, 0), radius, -transform.forward, out hitInfo, maxDistance, layerMask) || Physics.SphereCast(this.transform.position + new Vector3(0, 0.7f, 0), radius, -transform.right, out hitInfo, maxDistance, layerMask) || Physics.SphereCast(this.transform.position + new Vector3(0, 0.7f, 0), radius, transform.right, out hitInfo, maxDistance, layerMask);
        if (hitSomething && hitInfo.transform != this.transform)
        {
            //var nowPosXZ = transform.position;
            //nowPosXZ.y = 0;
            //var hitPointXZ = hitInfo.point;
            //hitPointXZ.y = 0;
            //var hitObject = hitInfo.transform.gameObject;
            //var concactVec = hitPointXZ - nowPosXZ;
            //nextPosition = transform.position - concactVec;
            //print(hitInfo.transform.name);
            nextPosition = Vector3.zero;//取消程式位移
            return hitSomethingWhenMoving;//回報碰撞，取消美術位移
            ////Npc 之間碰撞
            //if (hitObject.tag =="Npc")
            //{
            //    //移動幅度要縮小
            //    var hitObjectPosXZ = hitObject.transform.position;
            //    hitObjectPosXZ.y = 0;
            //    var backVec = hitObjectPosXZ - hitPointXZ;
            //    hitObject.transform.position += backVec;
            //    return false;
            //}
            //else//靜物碰撞
            //{
            //    var concactVec = hitPointXZ - nowPosXZ;
            //    nextPosition = transform.position - concactVec;
            //    return true;
            //}


        }
        return false;
    }

    void NpcCollision()
    {
        foreach (var item in ObjectManager.NpcsAlive.Values)
        {
            if (item == this) continue;

            var distance = Vector3.Distance(this.transform.position, item.transform.position);
            if (distance > 0.6) continue;

            var direction = (item.transform.position - this.transform.position).normalized;
            this.transform.position -= direction * 0.03f;
            item.transform.position += direction * 0.03f;
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
            grounded = !EasyFalling.Fall(transform, ref initVel, EndingYValue: terrainHeight);
            nextPosition = Vector3.zero;
        }

        SetAnimationGroundedParameter();
    }

    private void SetAnimationGroundedParameter()
    {
        var a = transform.GetComponent<Animator>();
        if (grounded && !a.GetBool("Grounded"))
        {
            a.SetBool("Grounded", UnityEngine.Random.value * UnityEngine.Random.value > 0.8f);
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
