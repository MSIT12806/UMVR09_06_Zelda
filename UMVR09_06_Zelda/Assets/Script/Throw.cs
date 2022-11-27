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
    public GameObject SwordEffect;
    private float Speed =0.25f;
    private float vertical =0.2f;
    private Vector3 Gravity = new Vector3(0,-1,0);

    Animator animator;
    public float coldTime = 10.0f;
    private float timer = 0.0f;
    private bool isStartTime = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanThrow==true) 
        {
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4))
            {
                GetThrowKeyIn();

                switch (useItem)
                {
                    case Item.TimeStop:
                        UseTimeStop();
                        break;
                    case Item.Ice:
                        UseIce();
                        break;
                    case Item.Bomb:
                        UseBomb();
                        break;
                }
            }
        }
        OnThrow(); //�L�̦b���ݭn���ɭ��٬O�@���QCALL�H
        CDTimer();
    }

    void GetThrowKeyIn()  //�������enum
    {
        if (Input.GetKeyDown(KeyCode.Alpha2)) useItem = Item.TimeStop;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) useItem = Item.Ice;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) useItem = Item.Bomb;

        Debug.Log(useItem);
    }

    void UseTimeStop() //�ϥήɰ�
    {
        isStartTime = true;
        Speed = 0.25f;
        vertical = 0.15f;
        animator.SetTrigger("ThrowTimeStop");
        CanThrow = false;
    }

    void UseIce() //�ϥΦB
    {
        isStartTime = true;
        Speed = 0.25f;
        vertical = 0.0f;
        animator.SetTrigger("ThrowIce");
        CanThrow = false;
    }

    void UseBomb() //�ϥά��u
    {
        isStartTime = true;
        Speed = 0.25f;
        vertical = 0.15f;
        animator.SetTrigger("ThrowBomb");
        CanThrow = false;
    }

    void CDTimer() //�p�ɾ�
    {
        if (isStartTime)
        {
            timer += Time.deltaTime;
            if (timer <coldTime)
            {
                CanThrow = false;
            }
            else if (timer >= coldTime)
            {
                CanThrow = true;
                timer = 0;
                isStartTime = false;
            }
        }
    }

    public void SwordFalse() //���C�����]�ʧ@�ƥ�^
    {
        SwordEffect.GetComponent<ParticleSystem>().Play();
        Sword.SetActive(false);
    }
    public void SwordTrue() //���C�X�{�]�ʧ@�ƥ�^
    {
        Sword.SetActive(true);
        SwordEffect.GetComponent<ParticleSystem>().Play();
    }

    public void GetBomb()  //���o���u�]�ʧ@�ƥ�^
    {
        var IK = this.GetComponent<IKController>();
        IK.Weight_Up = 0.0f;

        Object o = Resources.Load("TranslucentCrystal_Bomb");
        GameObject go = Instantiate((GameObject)o);
        go.transform.SetParent(RightHandThrow_pos.transform);
        go.transform.position = RightHandThrow_pos.position;
        ThrowItem = go.transform;
    }

    public void GetIce() //���o�B�]�ʧ@�ƥ�^
    {
        var IK = this.GetComponent<IKController>();
        IK.Weight_Up = 0.0f;

        Object o = Resources.Load("TranslucentCrystal_Ice");
        GameObject go = Instantiate((GameObject)o);
        go.transform.SetParent(RightHandThrow_pos.transform);
        go.transform.position = RightHandThrow_pos.position;
        ThrowItem = go.transform;
    }

    public void GetTimeStop()  //���o�ɰ��]�ʧ@�ƥ�^
    {
        var IK = this.GetComponent<IKController>();
        IK.Weight_Up = 0.0f;

        Object o = Resources.Load("TranslucentCrystal_TimeStop");
        GameObject go = Instantiate((GameObject)o);
        go.transform.SetParent(RightHandThrow_pos.transform);
        go.transform.position = RightHandThrow_pos.position;
        ThrowItem = go.transform;
    }

    public void StartThrow() //��X�ɪ����m��l�ơ]�ʧ@�ƥ�^
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

    public void OnThrow()  //�C�V����B��
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
    public void EndThrow() //����R��
    {
        Destroy(ThrowItem.gameObject);
        ThrowItem = null;
        isThrowing = false;
    }


    /*
    
    1.�ʧ@���p���D�G���A��
    2.�O�_���ӥ��C���q�έp�ɾ�
    3.�ɦ�
    4.UI

    �ɰ�()
    {
    �ʵe����
    IK����
    �D��i�JCD
    UI�ާ@
    �Ǫ�����
    }

    �S��&�P�w�G
    1.���o����̫᪺��m���� >> �j�b����W
    2.�T�w�b�H���e��Y�B�Ұ� >> �ʵe�ƥ�


     */

}
