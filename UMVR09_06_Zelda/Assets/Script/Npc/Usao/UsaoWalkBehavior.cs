using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 處理對峙時移動的碰撞問題
/// </summary>
public class UsaoWalkBehavior : StateMachineBehaviour
{
    Npc npc;
    bool awake = false;
    IKController ik;
    bool isNightScene;
    Transform target;
    Transform targetHead;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //awake
        if (awake == false)
        {
            ik = animator.GetComponent<IKController>();
            npc = animator.GetComponent<Npc>();
            //...

            awake = true;

            var currentScene = SceneManager.GetActiveScene();
            var currentSceneName = currentScene.name;
            isNightScene = currentSceneName == "NightScene";

            target = isNightScene ? ObjectManager.MainCharacter : ObjectManager2.MainCharacter;
            targetHead = isNightScene ? ObjectManager.MainCharacterHead : ObjectManager2.MainCharacterHead;
        }

        ik.LookAtObj = ObjectManager.MainCharacterHead;
        ik.IkActive = true;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (npc.collide)
        {
            animator.applyRootMotion = false;
        }
        else
        {
            animator.applyRootMotion = true;
            AiStateCommon.Turn(animator.transform, target.position - animator.transform.position);
        }

    }
}
