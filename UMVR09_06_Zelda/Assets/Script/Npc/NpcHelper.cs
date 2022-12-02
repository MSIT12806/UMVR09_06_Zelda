using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NpcHelper
{
    public float Hp { get; set; }
    public void GetHurt(DamageData damageData);
    public void Move();
    public void Turn(Vector3 direction);
    //public Vector3 OriginPosition { get; }
}
public class StateAnimationInfo
{
    public Action[] SetAnimations;
}
public class AttackStateInfo
{
    public float MinAttackDistance;
    public float MaxAttackDistance;
    public float Weight;
    public string TriggerName;

}
public class ChaseStateInfo
{
    public float KeepAwayDistance;
    public float ApproachDistance;
}
