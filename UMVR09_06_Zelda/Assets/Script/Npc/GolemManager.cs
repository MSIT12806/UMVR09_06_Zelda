using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemManager : MonoBehaviour, NpcHelper
{
    AiState aiState;
    Npc npc;
    public bool dizzy = false;
    public float Hp { get => npc.Hp; set => npc.Hp = value; }

    public float Shield = 0f;

    public bool CanBeKockedOut => false;

    public bool Dizzy => dizzy;

    public float MaxHp { get => npc.MaxHp; }

    public float WeakPoint => ((GolemBaseState)aiState).GetArmor();

    public float MaxWeakPoint => 12;

    public float Radius => 1.5f;

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

        //Debug.Log(Shield);
        //Debug.Log(Dizzy);
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
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 4f, false, new DamageData(10f, transform.forward * 0.6f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//
        if (attackType == 2)//技能2
        {
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 8f, false, new DamageData(10f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//
            //Once.CanMove = false;
        }
        if (attackType == 3)//技能1
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 5f, false, new DamageData(5f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//
        if (attackType == 4)//普攻2
            NpcCommon.AttackDetection("", transform.position, transform.forward, 90, 5f, false, new DamageData(5f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");

    }
    public void SetShield()
    {
        Shield = 10;
    }

    public void ArmorUi()
    {
        dizzy = true;
    }

    //public void OnDrawGizmos()
    //{
    //    Gizmos.DrawRay(transform.position, transform.forward*4f);
    //    //Gizmos.DrawRay(transform.position, transform.position + new Vector3(0, 0, 10));
    //}
}

