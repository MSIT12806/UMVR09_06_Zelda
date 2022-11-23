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
    private bool isThrowing = false;

    private Transform ThrowItem;
    public Transform RightHandThrow_pos;
    public GameObject Sword;
    public float Speed = 0.25f;
    public float vertical = 0.2f;
    private Vector3 Gravity = new Vector3(0,-1,0);

    // Update is called once per frame
    void Update()
    {
        if (isThrowing == true) 
        {
            OnThrow();
        }
    }
    public void SwordFalse()
    {
        Sword.SetActive(false);
    }

    public void GetBomb()
    {
        Object o = Resources.Load("TranslucentCrystal_Bomb");
        GameObject go = Instantiate((GameObject)o);
        go.transform.SetParent(RightHandThrow_pos.transform);
        go.transform.position = RightHandThrow_pos.position;
        ThrowItem = go.transform;
    }

    public void GetIce()
    {
        Object o = Resources.Load("TranslucentCrystal_Ice");
        GameObject go = Instantiate((GameObject)o);
        go.transform.SetParent(RightHandThrow_pos.transform);
        go.transform.position = RightHandThrow_pos.position;
        ThrowItem = go.transform;
    }

    public void StartThrow() 
    {
        isThrowing = true;
        ThrowItem.transform.parent = null;

        face = transform.forward;

        //��l��m = ����ͦ���m
        start_pos = ThrowItem.position;
        //��e��m = ��l��m
        current_pos = start_pos;

        //��l�t�� = �H�����V * �t�v
        start_vel = face * Speed;
        start_vel.y += vertical;
        //��e�t�� = ��l�t��
        vel = start_vel;

        Debug.Log(face);

        //���O = - ��l�t�� * 0.X
        resistance = -(vel) * 0.05f;
    }

    public void OnThrow() 
    {
        //�U�@��m = ��e��m + ��e�t��
        //��e��m = �U�@��m
        current_pos = ThrowItem.position + vel;
        //���鲾�ʨ��e��m
        ThrowItem.position = current_pos;

        //�U�@�t�� = ��e�t�� + ���O * Time.Deltatime + ���O * Time.Deltatime
        next_vel = vel + Gravity * Time.deltaTime + resistance * Time.deltaTime;
        //��e�t�� = �U�@�t��
        vel = next_vel;
    }
    public void EndThrow()
    {
        Destroy(ThrowItem.gameObject);
        ThrowItem = null;
        isThrowing = false;
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
