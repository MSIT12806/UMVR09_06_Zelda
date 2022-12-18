using Ron;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class ObjectManager2 : MonoBehaviour
{

    public static Dictionary<int, GameObject> NpcsAlive;//碰撞偵測、攻擊判定用。
    public static List<GameObject> Statics;
    public static Queue<GameObject> AttackFx;
    public static Dictionary<int, NpcHelper> StateManagers = new Dictionary<int, NpcHelper>();
    public static TPSCamera myCamera;
    public static Transform MainCharacter;
    public static Transform Elena;
    public static Transform MainCharacterHead;
    public static GameObject TimeStopChain;
    public Transform MyCharacter;
    public Transform Space;
    public static int[] StageMonsterMonitor { get; } = new int[4];




    //    public static List<GameObject> UsaoResources;
    //處理 npc 碰撞、偵測、迴避、群體運動等行為。
    //chase: 檢查目前會攻擊玩家的角色有幾人，並適時切換 npc 狀態為 around or close。
    private void Awake()
    {
        MainCharacter = MyCharacter;
        MainCharacterHead = MainCharacter.FindAnyChild<Transform>("Head");
        Elena = Space;

        //npc 池 初始化
        NpcsAlive = GameObject.FindGameObjectsWithTag("Npc").ToDictionary(i => i.GetInstanceID());
        GameObject.FindGameObjectsWithTag("Player").ToList().ForEach(i => NpcsAlive.Add(i.GetInstanceID(), i));

        //特效池
        AttackFx = new Queue<GameObject>(20);
        InitAttackFx();

        //載入短暫浮現的特效或物件
        TimeStopChain = Instantiate((GameObject)Resources.Load("FX_TimeStopKeep")); //時間暫停
        TimeStopChain.SetActive(false);
        //將管理物件隱藏
        var meshs = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (var item in meshs)
        {
            item.enabled = false;
        }

    }
    void InitAttackFx()
    {
        var fx = (GameObject)Resources.Load("CFXR Hit A (Red)");
        for (int i = 0; i < 20; i++)
        {
            var go = Instantiate(fx);
            AttackFx.Enqueue(go);
        }
    }

}
