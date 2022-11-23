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
    Collider collider;
    ThirdPersonCharacter tpc;

    Vector3 nextPosition;

    public bool collide;
    void Start()
    {
        nextPosition = this.transform.position;
        tpc = GetComponent<ThirdPersonCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        collide = StaticCollision();
        tpc.artistMovement = !collide;
    }
    void FixedUpdate()
    {


    }
    public bool StaticCollision(float radius = 0.23f, float maxDistance = 0.3f)
    {
        var hitSomething = Physics.SphereCast(this.transform.position + new Vector3(0, 0.7f, 0), radius,transform.forward, out var hitInfo, maxDistance, layerMask);
        if (hitSomething && hitInfo.transform != this.transform)
        {
            print("hit");
            var nowPosXZ = transform.position;
            nowPosXZ.y = 0;
            var hitPointXZ = hitInfo.point;
            hitPointXZ.y = 0;
            var hitObject = hitInfo.transform.gameObject;
            //Npc 之間碰撞
            if (hitObject.tag =="Npc")
            {
                var hitObjectPosXZ = hitObject.transform.position;
                hitObjectPosXZ.y = 0;
                var backVec = hitObjectPosXZ - hitPointXZ;
                hitObject.transform.position -= backVec;
                return false;
            }
            else//靜物碰撞
            {
                var concactVec = hitPointXZ - nowPosXZ;
                nextPosition = transform.position - concactVec;
                return true;
            }


        }
        return false;
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
        this.transform.Translate(damageData.force.point);
        //判定死亡
    }

    public float Hp { get; set; }

    private void OnDrawGizmos()
    {
        //檢查球射線是否有交點
        float sphereCastRadius = 0.23f;
        float range = 0.3f;
        var position = this.transform.position + new Vector3(0, 0.7f, 0);
        var direction = transform.forward;
        Gizmos.DrawWireSphere(position, sphereCastRadius);

        RaycastHit hit;
        if (Physics.SphereCast(position, sphereCastRadius, direction, out hit, range, layerMask) && hit.transform.gameObject.layer != this.gameObject.layer)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, 0.1f);
            //Debug.DrawLine(transform.position, sphereCastMidpoint, Color.green);
        }
        else
        {
            Gizmos.color = Color.red;
            Vector3 sphereCastMidpoint = position - direction * sphereCastRadius + direction * range;// + (direction * (range - sphereCastRadius));
            Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
            //Debug.DrawLine(transform.position, sphereCastMidpoint, Color.red);
        }
    }
}
