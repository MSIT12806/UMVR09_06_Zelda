using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType
{
    none,
    light,
    Heavy

}
public enum DamageState
{
    Normal,
    Bomb,
    Ice,
    TimePause,

}
public class DamageData
{
    public Transform Attacker;
    public static DamageData NoDamage = new DamageData(null, 0, HitType.none, DamageStateInfo.NormalAttack);
    public float Damage;
    public HitType Hit;
    public IEnumerable<DamageStateInfo> DamageStates;

    public DamageData(Transform attacker, float damage, HitType hitType, params DamageStateInfo[] hitInfluence)
    {
        Damage = damage;
        Hit = hitType;
        DamageStates = hitInfluence;
        Attacker = attacker;
    }
}
public class DamageStateInfo
{
    public static DamageStateInfo NormalAttack = new DamageStateInfo(DamageState.Normal, 0);
    public DamageState damageState;
    public float KeepTime;
    public DamageStateInfo(DamageState damageState, float keepSecond)
    {
        this.damageState = damageState;
        this.KeepTime = keepSecond;
    }
}
