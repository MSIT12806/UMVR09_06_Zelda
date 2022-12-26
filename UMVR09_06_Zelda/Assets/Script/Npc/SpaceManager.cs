using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ron;
using UnityEngine.Playables;
using Cinemachine;

public class SpaceManager : MonoBehaviour, NpcHelper
{
    public bool Show;//表演完設為true
    public HashSet<ParticleSystem> EffectPlaying = new HashSet<ParticleSystem>();
    Npc npc;
    public SpaceWeapon spaceWeapon;
    public List<GameObject> smallBallsAroundBody = new List<GameObject>();
    public float Hp { get => npc.Hp; set => npc.Hp = value; }

    public float MaxArmor = 12;
    public float Armor = 12;

    public bool dizzy = false;

    public bool CanBeKockedOut => throw new System.NotImplementedException();

    public bool Dizzy => dizzy;

    public float MaxHp => npc.MaxHp;

    public float WeakPoint => Armor;

    public float MaxWeakPoint => MaxArmor;

    public float Radius => 0.6f;

    public float CollisionDisplacement => 0;

    public string Name => "月影之主  阿蘭娜";

    public float FreezeTime = 0;

    public bool InSkill1State;
    public bool InSkill2State;
    public bool InSkill3State;
    public bool CanGetHit;

    public float ArmorBreakTime = 7;
    public float ShowWeakTime = 0;

    BlackFade1 BlackScreen;

    public PlayableDirector spaceEndDirector;
    public CinemachineVirtualCamera SpaceVirtualCamera;
    public CinemachineVirtualCamera SpaceVirtualCamera2;
    bool CanPlayEnd = true;
    public GameObject EndSpace;
    // Start is called before the first frame update
    Animator animator;
    void Awake()
    {
        //EndSpace = GameObject.Find("space3_3").gameObject;
        BlackScreen = GameObject.Find("BlackScreen").GetComponent<BlackFade1>();
        var currentScene = SceneManager.GetActiveScene();
        var currentSceneName = currentScene.name;
        if (currentSceneName == "NightScene")
        {
            ObjectManager.StateManagers.Add(this.gameObject.GetInstanceID(), this);
        }
        else
        {
            ObjectManager2.StateManagers.Add(this.gameObject.GetInstanceID(), this);
        }
        animator = transform.GetComponent<Animator>();
        npc = transform.GetComponent<Npc>();
    }
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        FreezeTime -= Time.deltaTime;
        if(FreezeTime <= 0)
        {
            foreach (var i in EffectPlaying)
            {
                if (i.isPaused)
                {
                    i.Play();
                }
            }
        }

        ShowWeakTime -= Time.deltaTime;

        if (Hp <= 0 && BlackScreen.newAlpha >= 1 && CanPlayEnd)
        {
            CanPlayEnd = false;
            //SpaceVirtualCamera.Priority = 20;
            EndSpace.SetActive(true);
            this.gameObject.SetActive(false);
            spaceEndDirector.Play();
        }

    }
    public void GetHurt(DamageData damageData)
    {
        if (Hp <= 0) return;
        Hp -= damageData.Damage;
        if (Hp <= 0)
        {
            Die();
            //BlackScreen.FadeOut(0.01f);
            return;
        }

        if (damageData.DamageState.damageState == DamageState.Finishing)
        {
            ArmorBreakTime = 0f;
            animator.Play("GetHit 0");
        }
        var currentAnimatorState = animator.GetCurrentAnimatorStateInfo(0);
        if (CanGetHit == true)//露出弱點
        {
            Armor -= 1;
            if (Armor <= 0 )
            {
                animator.Play("ArmorBreak");
            }
            else if(currentAnimatorState.IsName("Week") || currentAnimatorState.IsName("GetHit"))
                animator.Play("GetHit");
            else if (currentAnimatorState.IsName("ArmorBreak") || currentAnimatorState.IsName("GetHit 0"))
                animator.Play("GetHit 0");
        }


        if (InSkill2State)
        {
            if (damageData.DamageState.damageState == DamageState.TimePause)
            {
                if (transform.Find("BlackHoll").gameObject.activeSelf)
                {
                    transform.Find("BlackHoll").gameObject.SetActive(false);
                }
                InSkill2State = false;
                animator.Play("GetHit");
                ShowWeakTime = 10;
                foreach (var i in EffectPlaying)
                {
                    i.Stop();
                    i.Clear();
                }
            }
        }

        else if (damageData.DamageState.damageState == DamageState.TimePause)
        {
            FreezeTime = 5;
            foreach(var i in EffectPlaying)
            {
                i.Pause();
            }
        }

        if (InSkill3State)
        {
            if (damageData.DamageState.damageState == DamageState.Bomb || damageData.DamageState.damageState == DamageState.Fever)
            {
                InSkill3State = false;
                animator.Play("GetHit");
                //if (damageData.DamageState.damageState == DamageState.TimePause)
                //    ShowWeakTime = 10;
                //else if(damageData.DamageState.damageState == DamageState.Fever)
                ShowWeakTime = 5;
                foreach (var i in EffectPlaying)
                {
                    i.Stop();
                    i.Clear();
                }
            }
        }

        if(damageData.DamageState.damageState == DamageState.Fever)
        {
            animator.Play("GetHit");
            ShowWeakTime = 5;
            if (transform.Find("BlackHoll").gameObject.activeSelf)
            {
                transform.Find("BlackHoll").gameObject.SetActive(false);
            }
            foreach (var i in EffectPlaying)
            {
                i.Stop();
                i.Clear();
            }
        }


    }

    public void Move()
    {
        throw new System.NotImplementedException();
    }

    public void FaceTarget(Vector3 targetPosition, Transform selfTransform, float perFrameDegree)
    {
        if (FreezeTime >= 0) return;
        MyLookAt.Look(targetPosition, selfTransform, perFrameDegree);
    }

    public void Look(Transform target)
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        UiManager.singleton.HideTip();
        ObjectManager2.myCamera.SetDefault();
        ObjectManager2.myCamera.m_StareTarget[1] = null;
        animator.Play("Standing_React_Death_Right");
        //UiManager.singleton.Success();
    }

    public void AnimationAttack(int attackType)
    {

        if (attackType == 1)//普通攻擊1
        {
            spaceWeapon.BigBallAttack(transform.position + transform.right + Vector3.up);
        }
        if (attackType == 2)//普通攻擊2
        {
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 4, false, new DamageData(30, transform.forward * 0.6f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
        }
        if (attackType == 3)//普通攻擊3
        {
            //print(111);
            NpcCommon.AttackDetectionRectangle("", transform.position, transform.forward, transform.right, 4, 7, false, new DamageData(30, transform.forward * 0.6f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
        }
        if (attackType == 4)//普通攻擊4
        {
            SmallBall ball1 = transform.FindAnyChild<SmallBall>("SmallBall");
            SmallBall ball2 = transform.FindAnyChild<SmallBall>("SmallBall (1)");
            SmallBall ball3 = transform.FindAnyChild<SmallBall>("SmallBall (2)");
            SmallBall ball4 = transform.FindAnyChild<SmallBall>("SmallBall (3)");
            SmallBall ball5 = transform.FindAnyChild<SmallBall>("SmallBall (4)");
            SmallBall ball6 = transform.FindAnyChild<SmallBall>("SmallBall (5)");
            ball1.nowAttack = true;
            ball2.nowAttack = true;
            ball3.nowAttack = true;
            ball4.nowAttack = true;
            ball5.nowAttack = true;
            ball6.nowAttack = true;
        }
        if (attackType == 6)//技能2
        {
            NpcCommon.AttackDetection("", transform.position + transform.forward * 2.5f, transform.forward, 360, 5, false, new DamageData(80, transform.forward * 0.6f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
            this.transform.Find("BlackHoll").gameObject.SetActive(false);
        }
        if (attackType == 7)//技能3
        {
            NpcCommon.AttackDetection("", transform.position, transform.forward, 360, 10, false, new DamageData(80, transform.forward * 0.6f, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
        }
        //if(attackType == 8)//關掉小球
        //{
        //    SmallBall ball1 = transform.FindAnyChild<SmallBall>("SmallBall");
        //    SmallBall ball2 = transform.FindAnyChild<SmallBall>("SmallBall (1)");
        //    SmallBall ball3 = transform.FindAnyChild<SmallBall>("SmallBall (2)");
        //    SmallBall ball4 = transform.FindAnyChild<SmallBall>("SmallBall (3)");
        //    SmallBall ball5 = transform.FindAnyChild<SmallBall>("SmallBall (4)");
        //    SmallBall ball6 = transform.FindAnyChild<SmallBall>("SmallBall (5)");
        //    ball1.nowAttack = false;
        //    ball2.nowAttack = false;
        //    ball3.nowAttack = false;
        //    ball4.nowAttack = false;
        //    ball5.nowAttack = false;
        //    ball6.nowAttack = false;
        //}
    }
    public void BlackHollOn()
    {
        this.transform.Find("BlackHoll").gameObject.SetActive(true);
    }

    public void Turn(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }
}
