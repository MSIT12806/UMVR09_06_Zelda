using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceMoveBehavior : StateMachineBehaviour
{
    Transform target;
    bool awake;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (awake == false)
        {
            target = ObjectManager.MainCharacter;
            awake = true;
        }
    }

    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    /// <summary>
    ///  把順移的腳本寫在這兒
    /// </summary>
    public void Teleport()
    {
        //目標：
        //1. 不能順移出場地
        //2. 不能卡住或無限墜落
        //3. 不能讓 Lico 打不到的位置
    }
}
