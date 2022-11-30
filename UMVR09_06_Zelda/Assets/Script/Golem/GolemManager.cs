using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemManager : MonoBehaviour,IHp
{
    AiState aiState;
    Npc npc;
    public float Hp { get => npc.Hp; set => npc.Hp = value; }

    void Awake()
    {
        print(GetInstanceID());
        ObjectManager.StateManagers.Add(this.gameObject.GetInstanceID(), this);
    }
    // Start is called before the first frame update
    void Start()
    {
        aiState = new GolemIdleState(ObjectManager.MainCharacter, transform.GetComponent<Animator>(), transform, 50f);
        npc = transform.GetComponent<Npc>();
    }

    // Update is called once per frame
    void Update()
    {
        aiState.SetAnimation();
        aiState = aiState.SwitchState();
    }
    public void GetHurt(DamageData damageData)
    {
        var send = GetComponent<AiState>();
        send.GolemDamageData = damageData;
        send.getHit = damageData;
        //aiState = new UsaoHurtState(transform.GetComponent<Animator>(), transform, damageData);
    }
}

