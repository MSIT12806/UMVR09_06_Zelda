using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType
{
    none,
    light,
    Heavy,
    Ice

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
    public static DamageData NoDamage = new DamageData(0, Vector3.zero, HitType.none, DamageStateInfo.NormalAttack);
    public float Damage;
    public HitType Hit;
    public IEnumerable<DamageStateInfo> DamageStates;
    public Vector3 Force;

    public DamageData(float damage, Vector3 force, HitType hitType, params DamageStateInfo[] hitInfluence)
    {
        Damage = damage;
        Hit = hitType;
        DamageStates = hitInfluence;
        Force = force;
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
