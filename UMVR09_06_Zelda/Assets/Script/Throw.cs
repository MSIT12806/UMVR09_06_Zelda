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
    ��l��m = ����ͦ���m
    ��e��m = ��l��m
    ��l�t�� = �H�����V * �t�v
    ���O = - ��l�t�� * 0.X
    ��e�t�� = ��l�t��

    update

    �U�@��m = ��e��m + ��e�t��
    ��e��m = �U�@��m
    �U�@�t�� = ��e�t�� + ���O * Time.Deltatime - ���O * Time.Deltatime 
    ��e�t�� = �U�@�t��

     */

}
