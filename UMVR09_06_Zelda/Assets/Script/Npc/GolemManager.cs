using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemManager : MonoBehaviour, NpcHelper
{
    AiState aiState;
    Npc npc;
    public float Hp { get => npc.Hp; set => npc.Hp = value; }

    public bool CanBeKockedOut => false;

    public bool Dizzy => false;

    public float MaxHp { get => 500; }

public float WeakPoint => throw new System.NotImplementedException();

    public float MaxWeakPoint => throw new System.NotImplementedException();

    public float Radius =>1.5f;

    public float CollisionDisplacement => 0;

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
        aiState = new GolemIdleState(ObjectManager.MainCharacter, animator, transform, 12f, this);
    }

    // Update is called once per frame
    void Update()
    {
        aiState.SetAnimation();
        aiState = aiState.SwitchState();
    }
    public void GetHurt(DamageData damageData)
    {
        Debug.Log("hit"); 
        aiState.getHit = damageData;
        //aiState = new UsaoHurtState(transform.GetComponent<Animator>(), transform, damageData);
    }

    public void Move()
    {
        throw new System.NotImplementedException();
    }

    public void Turn(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }

    public void Look(Transform target)
    {
        throw new System.NotImplementedException();
    }
    public void AnimationAttack(int attackType)
    {
        if (attackType == 1)//普攻1
            NpcCommon.AttackDetection("",transform.position, transform.forward, 360, 4f, false, new DamageData(10f, transform.forward * 0.6f, HitType.Heavy,DamageStateInfo.NormalAttack), "Player");//
        if (attackType == 2)//技能2
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 8f, false, new DamageData(10f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//
        if (attackType == 3)//技能1
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 5f, false, new DamageData(5f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//
    }


    public void Skill1Attack(float AttackSpeed)//事件觸發
    {
        //if (AttackSpeed == 0.4) canMove = true;//攻擊位移開關
        //else canMove = false;
        Debug.Log("hiiiii");
        animator.SetFloat("Skill1AttackSpeed", AttackSpeed);
    }
    public void Skill2Attack(float AttackSpeed)//事件觸發
    {

        //if (AttackSpeed == 0.8) canMove = true;//攻擊位移開關
        //else canMove = false;

        //if (AttackSpeed == 0.2f)
        //{
        //    canInterrupt = true;
        //}
        //else
        //{
        //    canInterrupt = false;
        //}
        animator.SetFloat("Skill2AttackSpeed", AttackSpeed);
        // +可用時間暫停中斷技能
        // getHit.DamageState.damageState == DamageState.TimePause 
    }

    //public void Skill1Attack(float AttackSpeed)//事件觸發
    //{

    //    var a = GetComponent<GolemSkillState>();
    //    a.Skill1Attack(AttackSpeed);
    //}

    //public void Skill2Attack(float AttackSpeed)//事件觸發
    //{
    //    var a = GetComponent<GolemSkillState>();
    //    a.Skill2Attack(AttackSpeed);
    //}

    //public void OnDrawGizmos()
    //{
    //    Gizmos.DrawRay(transform.position, transform.forward*4f);
    //    //Gizmos.DrawRay(transform.position, transform.position + new Vector3(0, 0, 10));
    //}
}

