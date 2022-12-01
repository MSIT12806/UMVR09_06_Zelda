using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DragonManager : MonoBehaviour, NpcHelper
{
    AiState aiState;
    Npc npc;
    public float Hp { get => npc.Hp; set => npc.Hp = value; }

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
        aiState = new DragonIdleState(ObjectManager.MainCharacter, transform.GetComponent<Animator>(), transform,this);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        aiState.SetAnimation();
        aiState = aiState.SwitchState();
    }
    public void GetHurt(DamageData damageData)
    {
        Hp -= damageData.Damage;
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
    [SerializeField] Transform dragonWeapon;
    [SerializeField] Transform dragonMouth;
    [SerializeField] Transform dragonHead;
    //頭的前方 是 left...
    public void ShootFireBall()
    {
        //事件觸發
        //把球球從口部的位置發出
        var fireBall = dragonWeapon.GetComponentsInChildren<Transform>(true).FirstOrDefault(i => i.gameObject.activeSelf==false && i.tag == "FireBall");
        if (fireBall == null) return;
        fireBall.gameObject.SetActive(true);
        fireBall.transform.position = dragonMouth.position;
        //速度、方向
        var shootMagic = fireBall.GetComponent<ShootMagic>();
        shootMagic.force = -dragonHead.transform.right / 15;
        shootMagic.existSeconds = 2;
        //一段時間後爆炸/消失
    }
}
