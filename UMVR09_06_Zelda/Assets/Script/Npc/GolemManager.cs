using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemManager : MonoBehaviour, NpcHelper
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
        aiState = new GolemIdleState(ObjectManager.MainCharacter, animator, transform, 50f, this);
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
}

