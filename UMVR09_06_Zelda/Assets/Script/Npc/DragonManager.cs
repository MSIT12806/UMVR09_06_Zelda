using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DragonManager : MonoBehaviour, NpcHelper
{
    AiState aiState;
    Npc npc;
    public float Hp { get => npc.Hp; set => npc.Hp = value; }
    public float MaxHp => npc.MaxHp;

    public bool CanBeKockedOut => canBeKnockedOut;

    public bool Dizzy => dizzy;

    float weakPoint;
    public float WeakPoint { get => weakPoint; set => weakPoint = value; }

    public float MaxWeakPoint => npc.MaxHp / 20;

    bool canBeKnockedOut;
    bool dizzy;

    // Start is called before the first frame update
    Animator animator;
    void Awake()
    {
        ObjectManager.StateManagers.Add(this.gameObject.GetInstanceID(), this);
        animator = transform.GetComponent<Animator>();
        npc = transform.GetComponent<Npc>();
    }
    void Start()
    {
        aiState = new DragonIdleState(ObjectManager.MainCharacter, transform.GetComponent<Animator>(), transform, this);

    }
    void Update()
    {
        Move();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        aiState.SetAnimation();
        aiState = aiState.SwitchState();
    }
    bool flyState;
    public void GetHurt(DamageData damageData)
    {
        if (Hp <= 0)
        {
            animator.Play("Die");
            ObjectManager.myCamera.SetDefault();
        }
        if (canBeKnockedOut)
        {

            var dState = damageData.DamageState;
            if (dState.damageState == DamageState.Bomb)
            {
                animator.Play("Dizzy2");
                canBeKnockedOut = false;
                dizzy = true;
                flyState = false;
            }
        }
        var currentAnimation = animator.GetCurrentAnimatorStateInfo(0);
        if (flyState)
        {
            var dState = damageData.DamageState;
            if (dState.damageState == DamageState.Bomb)
            {
                Hp -= damageData.Damage;
            }

            return;
        }
        else
        {
            Hp -= damageData.Damage;
            //aiState = new UsaoHurtState(transform.GetComponent<Animator>(), transform, damageData);
        }

    }

    int moveType;
    public void SetMove(int m)
    {
        moveType = m;
    }

    public void Turn(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }
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
        shootMagic.force = (ObjectManager.MainCharacter.position - dragonHead.position).normalized / 15;
        shootMagic.existSeconds = 2;
        //一段時間後爆炸/消失
    }

    public void Look(Transform target)
    {
        throw new System.NotImplementedException();
    }

    public void Move()
    {

        switch (moveType)
        {
            case 1://walk
                transform.position += transform.forward * 0.05f;
                return;
            case 2://run
                transform.position += transform.forward * 0.10f;
                return;
            case 3://fly
                transform.position += transform.forward * 0.15f;
                return;
        }
    }

    public void SetKnockedOut(int notZero)
    {
        //弱點槽的UI
        canBeKnockedOut = notZero == 0;
    }

    #region Event
    public void Fly()  //Scream
    {
        print(MaxHp);
        print(Hp);
        flyState = Hp >= MaxHp / 2;
        animator.SetBool("Fly", flyState);
    }
    public void Land()
    {
        flyState = false;
    }
    public void DizzyEnd()
    {
        dizzy = false;
    }
    #endregion
}
