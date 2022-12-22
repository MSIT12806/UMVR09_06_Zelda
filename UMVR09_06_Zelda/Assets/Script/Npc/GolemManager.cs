using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ron;

public class GolemManager : MonoBehaviour, NpcHelper
{
    AiState aiState;
    Npc npc;
    GameObject apple;
    GameObject heart;

    public bool dizzy = false;

    public GameObject ShieldEffect;
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
    public bool Stand { get; set; }

    AudioSource FootL;
    AudioSource FootR;

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
        FootL = this.transform.FindAnyChild<Transform>("Foot_L").GetComponent<AudioSource>();
        FootR = this.transform.FindAnyChild<Transform>("Foot_R").GetComponent<AudioSource>();
        aiState = new GolemIdleState(ObjectManager.MainCharacter, animator, transform, 12f, this);


        ObjectManager.NpcsAlive.Remove(gameObject.GetInstanceID());
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        aiState.SetAnimation();
        aiState = aiState.SwitchState();

        ShieldEffectControl();

        Debug.DrawLine(transform.position, transform.position + transform.forward * 5f, Color.green);
        //Debug.Log(Shield);
        //Debug.Log(Dizzy);
    }
    public void GetHurt(DamageData damageData)
    {
        if (Hp <= 0)
        {
            UiManager.singleton.HideTip();
            return;
        }
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
        {
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 4.5f, false, new DamageData(30f, transform.forward * 0.6f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//
            this.transform.Find("FX_GolemAttack01").gameObject.GetComponent<ParticleSystem>().Play();
        }
        if (attackType == 2)//技能2
        {
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 8f, false, new DamageData(50f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//
            this.transform.Find("FX_GolemSkill0201").gameObject.GetComponent<ParticleSystem>().Play();
        }
        if (attackType == 5)//技能2的第二段傷害
        {
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 10f, false, new DamageData(50f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//
            this.transform.Find("FX_GolemSkill0202").gameObject.GetComponent<ParticleSystem>().Play();
        }
        if (attackType == 3)//技能1
        {
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 4f, false, new DamageData(50f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");//
            this.transform.Find("FX_GolemSkill01").gameObject.GetComponent<ParticleSystem>().Play();
        }
        if (attackType == 4)//普攻2 之後要改成長方形的攻擊判定
        {
            NpcCommon.AttackDetectionRectangle("", transform.position, transform.forward, transform.right, 2, 4, false, new DamageData(30f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
            //NpcCommon.AttackDetection("", transform.position, transform.forward, 90, 5f, false, new DamageData(30f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
        }
        if (attackType == 6)//普攻3 之後要改成長方形的攻擊判定
        {
            NpcCommon.AttackDetectionRectangle("", transform.position, transform.forward, transform.right, 3, 10, false, new DamageData(30f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
            //NpcCommon.AttackDetection("", transform.position, transform.forward, 90, 10f, false, new DamageData(30f, transform.forward * 0.3f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
            this.transform.Find("FX_GolemAttack03").gameObject.GetComponent<ParticleSystem>().Play();
        }

    }

    public void SoundController(int SoundType)
    {
        if(SoundType == 1)
        {
            FootL.Play();
        }
        if(SoundType == 2)
        {
            FootR.Play();
        }
    }

    void GolemAttack0201()
    {
        this.transform.Find("FX_GolemAttack0201").gameObject.GetComponent<ParticleSystem>().Play();
    }

    void GolemAttack0202()
    {
        this.transform.Find("FX_GolemAttack0202").gameObject.GetComponent<ParticleSystem>().Play();
    }

    public void SetShield()
    {
        Shield = 10;
        this.transform.Find("FX_Yell").gameObject.GetComponent<ParticleSystem>().Play();
    }

    public void ShieldEffectControl()
    {
        if (Shield > 0 && ShieldEffect.active == false)
        {
            ShieldEffect.SetActive(true);
        }
        else if(Shield <= 0 && ShieldEffect.active == true)
        {
            ShieldEffect.SetActive(false);
        }
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

