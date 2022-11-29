using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonManager : MonoBehaviour
{
    AiState aiState;
    // Start is called before the first frame update
    void Start()
    {
        aiState = new DragonIdleState(ObjectManager.MainCharacter, transform.GetComponent<Animator>(), transform);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        aiState.SetAnimation();
        aiState = aiState.SwitchState();
    }
    public void GetHurt(DamageData damageData)
    {
        //aiState = new UsaoHurtState(transform.GetComponent<Animator>(), transform, damageData);
    }
}
