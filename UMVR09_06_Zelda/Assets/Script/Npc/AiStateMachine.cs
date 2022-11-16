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
