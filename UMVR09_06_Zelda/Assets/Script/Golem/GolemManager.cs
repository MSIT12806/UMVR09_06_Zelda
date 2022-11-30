using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemManager : MonoBehaviour
{
    AiState aiState;
    // Start is called before the first frame update
    void Start()
    {
        aiState = new GolemIdleState(ObjectManager.MainCharacter, transform.GetComponent<Animator>(), transform, 50f);
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
        send.getHit = damageData;
        //aiState = new UsaoHurtState(transform.GetComponent<Animator>(), transform, damageData);
    }
}

