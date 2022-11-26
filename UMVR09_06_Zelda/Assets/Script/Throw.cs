using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    private enum Item 
    {
        Apple,
        TimeStop,
        Ice,
        Bomb,
    }

    private Vector3 start_pos;
    private Vector3 current_pos;
    private Vector3 start_vel;
    private Vector3 vel;
    private Vector3 next_vel;
    private Vector3 resistance;

    private Vector3 face;
    private bool CanThrow = true;
    private bool isThrowing = false;
    private Item useItem;

    private Transform ThrowItem;
    public Transform RightHandThrow_pos;
    public GameObject Sword;
    public float Speed = 0.25f;
    public float vertical = 0.2f;
    private Vector3 Gravity = new Vector3(0,-1,0);

    Animator animator;


    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanThrow==true) 
        {
            GetThrowKeyIn();

            switch (useItem) 
            {
                case Item.TimeStop:
                    useTimeStop();
                    break;
                case Item.Ice:
                    useIce();
                    break;
                case Item.Bomb:
                    useBomb();
                    break;
            }
        }
        OnThrow();
    }

    void GetThrowKeyIn() 
    {
        if (Input.GetKeyDown(KeyCode.Alpha2)) useItem = Item.TimeStop;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) useItem = Item.Ice;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) useItem = Item.Bomb;
    }

    void useTimeStop() 
    {
        animator.SetTrigger("ThrowTimeStop");
        CanThrow = false;
    }

    void useIce() 
    {
        animator.SetTrigger("ThrowIce");
        CanThrow = false;
    }

    void useBomb() 
    {
        animator.SetTrigger("ThrowBomb");
        CanThrow = false;
    }

    public void SwordFalse()
    {
        Sword.SetActive(false);
    }
    public void SwordTrue()
    {
        Sword.SetActive(true);
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

    public void GetTimeStop()
    {
        Object o = Resources.Load("TranslucentCrystal_TimeStop");
        GameObject go = Instantiate((GameObject)o);
        go.transform.SetParent(RightHandThrow_pos.transform);
        go.transform.position = RightHandThrow_pos.position;
        ThrowItem = go.transform;
    }

    public void StartThrow() 
    {
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

        //Debug.Log(face);

        //���O = - ��l�t�� * 0.X
        resistance = -(vel) * 0.05f;

        isThrowing = true;
    }

    public void OnThrow() 
    {
        if (isThrowing)
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
    }
    public void EndThrow()
    {
        Destroy(ThrowItem.gameObject);
        ThrowItem = null;
        isThrowing = false;
    }


    /*
    
    if (CanThrow)
    {
        switch(����)
        {
        case Alpha.2:�ɰ�
        ����ɰ�()
        case Alpha.3:�B
        ����B()
        case Alpha.2:���u
        ���欵�u()
        }
    }

    Ū������()
    CD�p�ɾ�()

    �ɰ�()
    {
    IK����
    �D��i�JCD
    UI�ާ@
    �Ǫ�����
    }
     */

}
