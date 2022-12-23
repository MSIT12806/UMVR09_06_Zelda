using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UsaoFightBehavior : StateMachineBehaviour
{
    bool awake = false;
    IKController ik;
    Npc npc;
    bool isNightScene;
    Transform target;
    Transform targetHead;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //awake
        if (awake == false)
        {
            var currentScene = SceneManager.GetActiveScene();
            var currentSceneName = currentScene.name;
            ik = animator.GetComponent<IKController>();
            npc = animator.GetComponent<Npc>();
            //...

            awake = true;
            isNightScene = currentSceneName == "NightScene";

            target = isNightScene ? ObjectManager.MainCharacter : ObjectManager2.MainCharacter;
            targetHead = isNightScene ? ObjectManager.MainCharacterHead : ObjectManager2.MainCharacterHead; 
        }

        ik.LookAtObj = targetHead;
        ik.IkActive = true;
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (npc.PauseTime > 0) return;
        AiStateCommon.Turn(animator.transform, target.position - animator.transform.position);
    }
}
