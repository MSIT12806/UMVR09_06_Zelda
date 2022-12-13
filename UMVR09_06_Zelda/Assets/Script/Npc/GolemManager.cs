using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemManager : MonoBehaviour, NpcHelper
{
    AiState aiState;
    Npc npc;
    GameObject apple;
    GameObject heart;
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

    public string Name => "克里曼魔像";

    // Start is called before the first frame update
    Animator animator;
    void Awake()
    {

        apple = (GameObject)Resources.Load("Apple");
        heart = (GameObject)Resources.Load("Obj_Heart");
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
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 4f, false, new DamageData(30f, transform.forward * 0.6f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//
        if (attackType == 2)//技能2
        {
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 8f, false, new DamageData(50f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//
            //Once.CanMove = false;
        }        
        if(attackType == 5)//技能2的第二段傷害
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 10f, false, new DamageData(50f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//

        if (attackType == 3)//技能1
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 4f, false, new DamageData(50f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//
        if (attackType == 4)//普攻2
            NpcCommon.AttackDetection("", transform.position, transform.forward, 90, 5f, false, new DamageData(30f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");

    }
    public void SetShield()
    {
        Shield = 10;
    }

    public void ArmorUi()
    {
        dizzy = true;
    }

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
        ObjectManager.myCamera.m_StareTarget[3] = null;
        UiManager.singleton.HideTip();

        //animator.Play("Die");
        var usaosBelongThirdStage = ObjectManager.StagePools[3];
        foreach (var item in usaosBelongThirdStage)
        {
            item.Die();
        }
    }

    //public void OnDrawGizmos()
    //{
    //    Gizmos.DrawRay(transform.position, transform.forward*4f);
    //    //Gizmos.DrawRay(transform.position, transform.position + new Vector3(0, 0, 10));
    //}
}

