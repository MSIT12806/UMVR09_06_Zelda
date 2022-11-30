using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NpcCommon
{
    public static void AttackDetection(Vector3 attackCenter, Vector3 attackForward, float repelDistance, float angle, float distance, float damage, HitType hitType, Vector3 force = default(Vector3))//攻擊範圍偵測
    {
        foreach (var item in ObjectManager.NpcsAlive.Values)
        {
            Transform nowNpc = item.transform;

            if (force == default(Vector3))
            {
                force = nowNpc.position - attackCenter;
            }
            force.y = 0;
            if (distance > Vector3.Distance(nowNpc.position.WithoutY(), attackCenter.WithoutY()))
            {
                force.Normalize();
                float fDot = Vector3.Dot(attackForward, force);
                if (fDot > 1) fDot = 1;
                if (fDot < -1) fDot = -1;

                float fThetaRadian = Mathf.Acos(fDot);
                float fThetaDegree = fThetaRadian * Mathf.Rad2Deg;
                if (fThetaDegree <= angle / 2)
                {
                    var attackReturn = nowNpc.gameObject.GetComponent<Npc>();
                    attackReturn.GetHurt(new DamageData(damage, force * repelDistance, hitType));
                }
            }
        }



        //print("Attack");
        //HashSet<Transform> hitInfoList = new HashSet<Transform>();
        //RaycastHit[] hitInfos;
        //for (int i = 0; i <= angle / 2; i += 5)
        //{

        //    hitInfos = Physics.RaycastAll(transform.position + (Vector3.up * 0.6f), Quaternion.Euler(0, i, 0) * transform.forward, distance, LY);//1 << LayerMask.NameToLayer("NPC")
        //    for (int j = 0; j < hitInfos.Length; j++)
        //    {

        //        //if(hitInfos[j].transform.tag == "Npc")
        //        hitInfoList.Add(hitInfos[j].transform);
        //    }

        //    hitInfos = Physics.RaycastAll(transform.position + (Vector3.up * 0.6f), Quaternion.Euler(0, -i, 0) * transform.forward, distance, LY);
        //    for (int j = 0; j < hitInfos.Length; j++)
        //    {
        //        //if (hitInfos[j].transform.tag == "Npc")
        //        hitInfoList.Add(hitInfos[j].transform);
        //    }
        //}
        //if (hitInfoList.Count > 0)
        //{
        //    foreach (Transform i in hitInfoList)
        //    {
        //        var attackReturn = i.gameObject.GetComponent<Npc>();
        //        print(i.transform);
        //        attackReturn.GetHurt(new DamageData(transform, 10f, HitType.light));
        //    }
        //}
    }

}
