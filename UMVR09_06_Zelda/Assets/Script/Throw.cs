using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    private Vector3 start_pos;
    private Vector3 current_pos;
    private Vector3 start_vel;
    private Vector3 vel;
    private Vector3 next_vel;
    private Vector3 resistance;

    private Vector3 face;


    public Transform ThrowItem;
    public Transform RightHandThrow_pos;
    public GameObject Sword;
    public float Speed = 0.1f;
    public float vertical = 1.0f;
    public Vector3 Gravity = Physics.gravity;

    // Start is called before the first frame update
    void Start()
    {
        face = transform.forward;

        //初始位置 = 物件生成位置
        start_pos = ThrowItem.position;
        //當前位置 = 初始位置
        current_pos = start_pos;
       

        //初始速度 = 人物面向 * 速率
        start_vel = face * Speed;
        start_vel.y += vertical;
        //當前速度 = 初始速度
        vel = start_vel;

        //阻力 = - 初始速度 * 0.X
        resistance = -(vel) * 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     public void OnThrow() 
    {
        ThrowItem.transform.parent = null;
        //下一位置 = 當前位置 + 當前速度
        //當前位置 = 下一位置
        current_pos = ThrowItem.position + vel;
        //物體移動到當前位置
        ThrowItem.position = current_pos;

        //下一速度 = 當前速度 + 重力 * Time.Deltatime + 阻力 * Time.Deltatime
        next_vel = vel + Gravity * Time.deltaTime + resistance * Time.deltaTime;
        //當前速度 = 下一速度
        vel = next_vel;
    }

    public void GetBomb() 
    {
        Sword.SetActive(false);
        Object o = Resources.Load("TranslucentCrystal_Bomb");
        GameObject go = Instantiate((GameObject)o);
        go.transform.SetParent(RightHandThrow_pos.transform);
        go.transform.position = RightHandThrow_pos.position;
        ThrowItem = go.transform;
    }

    /*
    初始位置 = 物件生成位置
    當前位置 = 初始位置
    初始速度 = 人物面向 * 速率
    阻力 = - 初始速度 * 0.X
    當前速度 = 初始速度

    update

    下一位置 = 當前位置 + 當前速度
    當前位置 = 下一位置
    下一速度 = 當前速度 + 重力 * Time.Deltatime - 阻力 * Time.Deltatime 
    當前速度 = 下一速度

     */

}
