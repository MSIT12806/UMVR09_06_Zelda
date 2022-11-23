using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    private Vector3 start_pos;
    private Vector3 current_pos;
    private Vector3 next_pos;
    public float Speed = 0.1f;
    private Vector3 start_vel;
    private Vector3 vel;
    private Vector3 next_vel;
    private Vector3 resistance;

    private Quaternion face;


    public GameObject ThrowItem;
    private Transform m_ThrowItem;

    // Start is called before the first frame update
    void Start()
    {
        face = transform.rotation;

        m_ThrowItem = ThrowItem.GetComponent<Transform>();
        
        start_pos = m_ThrowItem.position;
        current_pos = start_pos;

        //start_vel = (0.0f,0.0f,face.eulerAngles.x) * Speed;
        vel = start_vel;

        resistance = -(m_ThrowItem.forward) * 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        next_pos = current_pos + vel;
        current_pos = next_pos;
        m_ThrowItem.position = current_pos;

        //next_vel = vel + Physics.gravity * Time.deltaTime - resistance * Time.deltaTime;
        next_vel = vel - resistance * Time.deltaTime;
        vel = next_vel;
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
