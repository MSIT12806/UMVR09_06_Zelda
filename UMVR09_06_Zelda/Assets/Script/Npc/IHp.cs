using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHp
{
    public float Hp { get; set; }
    public void GetHurt(DamageData damageData);
}
