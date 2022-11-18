using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BasicAi
{
    //Ai 的基本構成
    // 1. 感官
    // 2. 表達
    // 3. 任務(狀態)
}
public abstract class AiState
{

    public abstract AiState SwitchState();
}
public class Idel : AiState
{
    Vector3 originPosition;
    //Idle的啟動要是固定範圍 Physics.CheckSphere

    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// 面向玩家
/// </summary>
public class Alert : AiState
{
    //固定範圍內展開追擊 Physics.CheckSphere
    GameObject alertTarget;
    public Alert(GameObject alertObject)
    {
        alertTarget = alertObject;
    }
    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
public class Taunt : AiState
{
    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// 朝著角色方向移動
/// </summary>
public class Chase : AiState
{
    ChaseState chaseState;
    enum ChaseState
    {
        Around,//todo: 要定範圍外停住
        Close  //貼近 8隻
    }
    GameObject alertTarget;
    public Chase(GameObject alertObject)
    {
        alertTarget = alertObject;
    }
    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
    public void AroundOrClose()
    {

    }
    public string GetChaseState()
    {
        return chaseState.DisplayName();
    }
    public void Turn() { }
    public void Seek() { }
}
/// <summary>
/// 停下並站在角色面前
/// </summary>
public class Confrontation : AiState
{
    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}
public  class Stiff_1 : AiState
{
    AiState recoverState = new Confrontation();
    AiState stiff_2 = new Stiff_2();
    float recoverSeconds = 5;
    public bool hitAgain;
    public Stiff_1()
    {
    }
    public AiState Recover()
    {
        recoverSeconds -= Time.deltaTime;
        return recoverSeconds < 0 ? recoverState : this;
    }
    public void Beaten()
    {
        hitAgain = true;
    }

    public override AiState SwitchState()
    {
        if (hitAgain) return stiff_2;
        return Recover();
    }
}

internal class Stiff_2 : AiState
{
    public override AiState SwitchState()
    {
        throw new NotImplementedException();
    }
}

//用腳本的方式控制動作狀態？


//警戒巡邏 //地域性

//攻擊
//1. 腳色進入領地 2. Npc 看見腳色

//追進
//對峙
//逃開

//受攻僵直1
//受攻僵直2
//受攻僵直3
//受攻僵直4

//擊倒
//起身
