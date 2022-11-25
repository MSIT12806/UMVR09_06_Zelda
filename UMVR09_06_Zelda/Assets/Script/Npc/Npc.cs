using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(Collider))]
public class Npc : MonoBehaviour, IHp
{
    // Start is called before the first frame update

    /*
     * 1. 對 collider 的碰撞偵測與碰撞反應。  記得問老師怎麼處理 npc碰撞 (raycast? 兩兩算距離?)
     * 1.1 幫每個物件設定半徑，畫raycast
     * 1.2 輪巡(容器？)幫每個物件算距離
     */
    [SerializeField] LayerMask layerMask;
    [SerializeField] LayerMask terrainLayer;
    Collider collider;

    Vector3 nextPosition;

    public bool collide;
    public bool OnGround;
    void Start()
    {
        nextPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        OnGround = StandOnTerrain();
        collide = StaticCollision();
        NpcCollision();
    }
    void FixedUpdate()
    {


    }


    public DamageData Attack()
    {
        //請善用狀態機處理攻擊判定
        return DamageData.NoDamage;
    }
    public void GetHurt(DamageData damageData)
    {
        //播放受傷僵直動畫
        //計算後退 or 擊飛方向 & 力道
        this.transform.Translate(-damageData.Attacker.position);
        print("GetHit");

        var u = transform.GetComponent<UsaoManager>();
        u.GetHurt(damageData);
        //判定死亡
    }
    [SerializeField] float hp;
    public float Hp { get { return hp; } set { hp = value; } }


    bool StaticCollision(float radius = 0.23f, float maxDistance = 0.3f)
    {
        var hitSomething = Physics.SphereCast(this.transform.position + new Vector3(0, 0.7f, 0), radius, transform.forward, out var hitInfo, maxDistance, layerMask);
        if (hitSomething && hitInfo.transform != this.transform)
        {
            var nowPosXZ = transform.position;
            nowPosXZ.y = 0;
            var hitPointXZ = hitInfo.point;
            hitPointXZ.y = 0;
            var hitObject = hitInfo.transform.gameObject;
            var concactVec = hitPointXZ - nowPosXZ;
            nextPosition = transform.position - concactVec;
            print(hitInfo.transform.name);
            return true;
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
        foreach (var item in ObjectManager.Npcs)
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



/*拋物線*/

public class Throweeee
{
    public void Go()
    {
        //拋物線速度公式：Vt = V0 + Gt 
        //先求 V0

        //Gt
        //Vt
        //拋物線位移 S = S0 + Vt
    }
}