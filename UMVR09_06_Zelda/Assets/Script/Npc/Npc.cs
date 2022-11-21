using System.Collections;
using System.Collections.Generic;
using UnityEngine;




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

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DebugExtension.DebugWireSphere(this.transform.position + new Vector3(0, 1.1f, 0));
        var hitSomething = Physics.SphereCast(this.transform.position + new Vector3(0, 1.1f, 0), 1f, this.transform.forward, out var result, layerMask);
        if (hitSomething)
        {
            //print(result.transform.name);
        }
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
    [Range(0.1f, 1f)] public float sphereCastRadius;
    [Range(0f, 100f)] public float range;

    public float Hp { get; set; }

    private void OnDrawGizmos()
    {
        var position = this.transform.position + new Vector3(0, 1.1f, 0);
        Gizmos.DrawWireSphere(position, sphereCastRadius);

        RaycastHit hit;
        if (Physics.SphereCast(position, sphereCastRadius, -transform.up * range, out hit, range, layerMask))
        {
            Gizmos.color = Color.green;
            Vector3 sphereCastMidpoint = transform.position + (transform.forward * hit.distance);
            Gizmos.DrawWireSphere(sphereCastMidpoint, 0.1f);
            Gizmos.DrawSphere(hit.point, 0.1f);
            //Debug.DrawLine(transform.position, sphereCastMidpoint, Color.green);
        }
        else
        {
            Gizmos.color = Color.red;
            Vector3 sphereCastMidpoint = position + (transform.forward * (range - sphereCastRadius));
            Gizmos.DrawWireSphere(sphereCastMidpoint, sphereCastRadius);
            //Debug.DrawLine(transform.position, sphereCastMidpoint, Color.red);
        }
    }
}
