using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DragonManager : MonoBehaviour, NpcHelper
{
    Npc npc;
    public float Hp { get => npc.Hp; set => npc.Hp = value; }
    public float MaxHp => npc.MaxHp;
    public bool Show;//表演完設為true

    public bool CanBeKockedOut => canBeKnockedOut;

    public bool Dizzy => dizzy;

    float weakPoint;
    public float WeakPoint { get => weakPoint; set => weakPoint = value; }

    internal void DizzyStart()
    {
        dizzy = true;
    }

    public float MaxWeakPoint => 12;

    public float Radius => 1.8f;

    public float CollisionDisplacement => 0;

    public Vector3 ArrivePoint { get; set; }

    public string Name => "克圖格亞";

    public bool fireballShooting;
    public bool sprinting;

    public bool canBeKnockedOut;
    bool dizzy;

    // Start is called before the first frame update
    Animator animator;
    void Awake()
    {
        if (name == "Blue Variant 2") return;
        ObjectManager.StateManagers.Add(this.gameObject.GetInstanceID(), this);  
        animator = transform.GetComponent<Animator>();
        npc = transform.GetComponent<Npc>();
        apple = (GameObject)Resources.Load("Apple");
        heart = (GameObject)Resources.Load("Obj_Heart");
    }
    void Start()
    {
        if (animator.name == "Blue Variant 2") return;
        weakPoint = MaxWeakPoint;
        gameObject.SetActive(false);
        ObjectManager.NpcsAlive.Remove(gameObject.GetInstanceID());
    }
    void Update()
    {
    }
    // Update is called once per frame
    void FixedUpdate()
    {

    }
    public void StartFly()
    {
        flyState = true;
    }
    public void EndFly()
    {
        flyState = true;
    }
    public bool flyState;
    public void GetHurt(DamageData damageData)
    {
        if (Hp <= 0) return;
        var dState = damageData.DamageState;
        if (flyState)
        {
            if (dState.damageState == DamageState.Bomb || dState.damageState == DamageState.Fever)
            {
                flyState = false;
                Hp -= damageData.Damage;
            }
        }
        else
        {
            Hp -= damageData.Damage;
        }
        if (Hp <= 0)
        {
            Die();
            return;
        }
        if (dState.damageState == DamageState.Fever)
        {
            animator.CrossFade("Dizzy2", 0.1f);
            canBeKnockedOut = false;
            dizzy = true;
            flyState = false;
        }

        if (canBeKnockedOut && fireballShooting)
        {
            if (dState.damageState == DamageState.Bomb)
            {
                animator.CrossFade("Dizzy2", 0.1f);
                canBeKnockedOut = false;
                dizzy = true;
                flyState = false;
            }
        }
        if (canBeKnockedOut && sprinting)
        {
            if (dState.damageState == DamageState.Ice)
            {
                animator.CrossFade("Dizzy2", 0.1f);
                canBeKnockedOut = false;
                dizzy = true;
                flyState = false;
            }
        }
        if (dizzy)
        {
            weakPoint--;
            if (weakPoint <= 0)
            {
                animator.CrossFade("ArmorBreak", 0.1f);
            }
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ArmorBreak"))
        {
            if (damageData.Hit == HitType.finishing)
            {
                animator.CrossFade("Get Hit 2", 0.25f, 0);
                ResetWeakPoint();
            }
        }
    }

    public void ResetWeakPoint()
    {

        dizzy = false;
        weakPoint = MaxWeakPoint;
    }

    private int moveFrame;
    int moveType;


    public void Turn(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }


    public void Look(Transform target)
    {
        throw new System.NotImplementedException();
    }

    public void Move()
    {
        if (npc.collideFront) return;
        if (moveFrame > 0)
        {
            moveFrame--;
            switch (moveType)
            {
                case 1://walk
                    transform.position += transform.forward * 0.05f;
                    return;
                case 2://run
                    transform.position += transform.forward * 0.10f;
                    return;
                case 3://fly
                    transform.position += transform.forward * 0.3f;
                    return;
            }
        }
    }

    public void SetKnockedOut(int notZero)
    {
        //弱點槽的UI
        canBeKnockedOut = notZero == 0;
    }

    #region Event
    [SerializeField] Transform dragonWeapon;
    [SerializeField] Transform dragonMouth;
    [SerializeField] Transform dragonHead;
    //頭的前方 是 left...
    public void ShootFireBall()
    {
        //事件觸發
        //把球球從口部的位置發出
        var fireBall = dragonWeapon.GetComponentsInChildren<Transform>(true).FirstOrDefault(i => i.gameObject.activeSelf == false && i.tag == "FireBall");
        if (fireBall == null) return;
        fireBall.gameObject.SetActive(true);
        fireBall.transform.position = dragonMouth.position;
        //速度、方向
        var shootMagic = fireBall.GetComponent<ShootMagic>();
        shootMagic.force = -dragonHead.transform.right;
        shootMagic.existSeconds = 2;
        //一段時間後爆炸/消失
    }

    public void Land()
    {
        flyState = false;
    }
    public void DizzyEnd()
    {
        dizzy = false;
    }

    public void TailAttack()
    {
        NpcCommon.AttackDetection("Dragon", transform.position, transform.forward, /*15*/360f, 8, false, new DamageData(30, Vector3.zero, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
    }
    GameObject apple;
    GameObject heart;

    public void Die()
    {
        //掉蘋果跟掉愛心
        int heartCount = UnityEngine.Random.Range(1, 3);
        for (int i = 0; i < heartCount; i++)
        {
            var go = Instantiate(heart);
            go.transform.position = transform.position + Vector3Extension.GetRandomDirection().AddY(1).normalized;

        }
        int appleCount = UnityEngine.Random.Range(2, 5);
        for (int i = 0; i < heartCount; i++)
        {
            var go = Instantiate(apple);
            go.transform.position = transform.position + Vector3Extension.GetRandomDirection().AddY(1).normalized;
        }

        ObjectManager.myCamera.SetDefault();
        ObjectManager.myCamera.m_StareTarget[2] = null;
        dizzy = false;
        UiManager.singleton.HideTip();
        animator.Play("Die");
        //把小怪都殺死
        var usaosBelongSecondStage = ObjectManager.StagePools[2];
        foreach (var item in usaosBelongSecondStage)
        {
            item.Die();
        }


    }
    #endregion
}
