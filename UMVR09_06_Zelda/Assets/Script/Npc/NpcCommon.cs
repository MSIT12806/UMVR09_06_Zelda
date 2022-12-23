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
        Queue<GameObject> attackFxs;
        if (isNightScene)
        {
            attackFxs = ObjectManager.AttackFx;
            lst = ObjectManager.NpcsAlive.Values.Where(i => tags.Contains(i.tag));
        }
        else
        {
            attackFxs = ObjectManager2.AttackFx;
            lst = ObjectManager2.NpcsAlive.Values.Where(i => tags.Contains(i.tag));
        }


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
                    var fx = attackFxs.Dequeue();
                    if (fx != null)
                    {
                        fx.transform.position = item.transform.position.AddY(1);
                        fx.SetActive(true);
                        fx.GetComponent<ParticleSystem>().Play();
                        attackFxs.Enqueue(fx);
                    }
                    var attackReturn = nowNpc.gameObject.GetComponent<Npc>();
                    attackReturn.GetHurt(damageData);

                    if (attacker == "Pico")
                    {
                        if (damageData.DamageState.damageState != DamageState.Fever && damageData.DamageState.damageState != DamageState.Other) PicoManager.Power++;
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
            bool inAttackRange = false;
            bool inAttackRange2 = false;

            Transform nowNpc = item.transform;
            var vec = nowNpc.position - attackCenter;
            vec.y = 0;
            float Dot = Vector3.Dot(attackForward, vec);
            if (Dot >= 0)
            {
                float forwardProject = Vector3.Project(vec, attackForward).magnitude;
                float RightProject = Vector3.Project(vec, attackRight).magnitude;

                if (Mathf.Abs(RightProject) <= Width / 2) inAttackRange = true;

                if (Mathf.Abs(forwardProject) <= distance) inAttackRange2 = true;
            }
            if (inAttackRange && inAttackRange2)
            {
                if (!repelDirection)
                {
                    damageData.Force = vec.normalized * 0.15f;
                }

                var attackReturn = nowNpc.gameObject.GetComponent<Npc>();
                attackReturn.GetHurt(damageData);
            }
        }
    }
}
