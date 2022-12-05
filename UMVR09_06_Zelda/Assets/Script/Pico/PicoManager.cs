using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PicoManager 
{

    public static float Hp { get => _hp; set { _hp = value > MaxHp ? MaxHp : value; } }
    static float _hp;
    public static float MaxHp { get; } = 1000;
    public static float Power { get => _power; set { _power = value > MaxPower ? MaxPower : value; } }
    static float _power;
    public static float MaxPower { get; } = 200;
    public static readonly float PowerCost = 100;

    static PicoManager()
    {
        _hp = MaxHp;
    }

}
