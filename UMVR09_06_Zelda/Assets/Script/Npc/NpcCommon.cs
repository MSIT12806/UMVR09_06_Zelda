using Ron;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class NpcCommon
{
    static bool isNightScene;
    static bool awake;

    public static void AttackDetection(string attacker, Vector3 attackCenter, Vector3 attackForward, float angle, float distance, bool repelDirection, DamageData damageData, params string[] tags)//攻擊範圍偵測
    {
        if (awake == false)
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
            awake = true;
        }

        IEnumerable<GameObject> lst = null;
        if (isNightScene)
            lst = ObjectManager.NpcsAlive.Values.Where(i => tags.Contains(i.tag));
        else
            lst = ObjectManager2.NpcsAlive.Values.Where(i => tags.Contains(i.tag));


        foreach (var item in lst)
        {
            Transform nowNpc = item.transform;

            var vec = nowNpc.position - attackCenter;

            if (distance > Vector3.Distance(nowNpc.position.WithY(), attackCenter.WithY()))
            {

                vec.Normalize();
                float fDot = Vector3.Dot(attackForward, vec.WithY());
                if (fDot > 1) fDot = 1;
                if (fDot < -1) fDot = -1;

                float fThetaRadian = Mathf.Acos(fDot);
                float fThetaDegree = fThetaRadian * Mathf.Rad2Deg;

                //擊中
                if (fThetaDegree <= angle / 2)
                {
                    //擊退位移(如果有預設就用預設，沒有就用中心推開)
                    if (!repelDirection)
                    {
                        damageData.Force = vec.normalized * 0.15f;
                    }
                    //擊中特效 還沒綁在所有物件上

                    var attackReturn = nowNpc.gameObject.GetComponent<Npc>();
                    attackReturn.GetHurt(damageData);
                    if (attacker == "Pico")
                    {
                        if (damageData.Hit != HitType.fever) PicoManager.Power++;
                        if (ObjectManager.AttackFx == null) return;
                        var fx = ObjectManager.AttackFx.Dequeue();
                        if (fx != null)
                        {
                            fx.transform.position = item.transform.position.AddY(1);
                            fx.SetActive(true);
                            fx.GetComponent<ParticleSystem>().Play();
                            ObjectManager.AttackFx.Enqueue(fx);
                        }
                    }

                }
            }
        }

    }

    public static void AttackDetectionRectangle(string attacker, Vector3 attackCenter, Vector3 attackForward, Vector3 attackRight, float Width, float distance, bool repelDirection, DamageData damageData, params string[] tags)//攻擊範圍偵測
    {

        if (awake == false)
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
            awake = true;
        }

        IEnumerable<GameObject> lst = null;
        if (isNightScene)
            lst = ObjectManager.NpcsAlive.Values.Where(i => tags.Contains(i.tag));
        else
            lst = ObjectManager2.NpcsAlive.Values.Where(i => tags.Contains(i.tag));


        foreach (var item in lst)
        {
            Transform nowNpc = item.transform;
            var vec = nowNpc.position - attackCenter;
            vec.Normalize();
            Vector3 cornor1 = attackCenter + attackForward * distance + attackRight * (Width / 2);
            Vector3 cornor2 = attackCenter + attackRight * -(Width / 2);

            bool inAttackRange = Mathf.Max(cornor1.x, cornor2.x) > nowNpc.position.x && Mathf.Min(cornor1.x, cornor2.x) < nowNpc.position.x;
            bool inAttackRange2 = Mathf.Max(cornor1.z, cornor2.z) > nowNpc.position.z && Mathf.Min(cornor1.z, cornor2.z) < nowNpc.position.z;
            if (inAttackRange && inAttackRange2)
            {
                if (!repelDirection)
                {
                    damageData.Force = vec.normalized * 0.15f;
                }

                var attackReturn = nowNpc.gameObject.GetComponent<Npc>();
                attackReturn.GetHurt(damageData);
                //if (attacker == "Pico")
                //{
                //    PicoManager.Power++;
                //    if (ObjectManager.AttackFx == null) return;
                //    var fx = ObjectManager.AttackFx.Dequeue();
                //    if (fx != null)
                //    {
                //        fx.transform.position = item.transform.position.AddY(1);
                //        fx.SetActive(true);
                //        fx.GetComponent<ParticleSystem>().Play();
                //        ObjectManager.AttackFx.Enqueue(fx);
                //    }
                //}
            }
        }
    }
}
