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
    Burn,
    Freeze,
    Fear,
    Poisoned
}
public class DamageData
{
    public Transform attacker;
    public static DamageData NoDamage = new DamageData(null, 0, HitType.none, DamageStateInfo.NormalAttack);
    public float Damage;
    public HitType hit;
    public IEnumerable<DamageStateInfo> damageStates;

    public DamageData(Transform attacker, float damage, HitType hitType, params DamageStateInfo[] hitInfluence)
    {
        Damage = damage;
        hit = hitType;
        damageStates = hitInfluence;
        this.attacker = attacker;
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
