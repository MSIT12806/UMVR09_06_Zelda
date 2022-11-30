using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NpcHelper 
{
    public float Hp { get; set; }
    public void GetHurt(DamageData damageData);
    public void Move();
    public void Turn(Vector3 direction);
}
