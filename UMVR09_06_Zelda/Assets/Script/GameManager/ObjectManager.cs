using Ron;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class ObjectManager : MonoBehaviour
{
    public static Dictionary<int, GameObject> NpcsAlive = new Dictionary<int, GameObject>();//碰撞偵測、攻擊判定用。
    public static Dictionary<int, GameObject>[] StageDeathPool = new Dictionary<int, GameObject>[4]
    {
        new Dictionary<int, GameObject>(),
        new Dictionary<int, GameObject>(),
        new Dictionary<int, GameObject>(),
        new Dictionary<int, GameObject>()
    };//復活用
    public static HashSet<Npc>[] StagePools = new HashSet<Npc>[4]
    {
        new HashSet<Npc>(), new HashSet<Npc>(), new HashSet<Npc>(), new HashSet<Npc>()
    };
    public static List<GameObject> Statics;
    public static Queue<GameObject> AttackFx;
    public static Queue<GameObject> DieFx;
    public static Queue<GameObject> BornFx;
    public static Dictionary<int, NpcHelper> StateManagers = new Dictionary<int, NpcHelper>();
    public static HashSet<AiState> ChasingNpc;
    public static TPSCamera myCamera;
    public static Transform MainCharacter;
    public static Transform MainCharacterHead;
    public static GameObject DragonFireBallExplosionFx;
    public static GameObject TimeStopChain;
    public static Transform stageOneSpawnPoint;
    public static Transform stageTwoSpawnPoint;
    public static Transform stageThreeSpawnPoint;
    public Transform MyCharacter;
    public static int[] StageMonsterMonitor { get; } = new int[4];




    //    public static List<GameObject> UsaoResources;
    //處理 npc 碰撞、偵測、迴避、群體運動等行為。
    //chase: 檢查目前會攻擊玩家的角色有幾人，並適時切換 npc 狀態為 around or close。
    private void Awake()
    {
        MainCharacter = MyCharacter;
        MainCharacterHead = MainCharacter.FindAnyChild<Transform>("Head");
        stageOneSpawnPoint = transform.FindAnyChild<Transform>("StageOneSpawnPoint");
        stageTwoSpawnPoint = transform.FindAnyChild<Transform>("StageTwoSpawnPoint");
        stageThreeSpawnPoint = transform.FindAnyChild<Transform>("StageThreeSpawnPoint");

        //npc 池 初始化
        //NpcsAlive = GameObject.FindGameObjectsWithTag("Npc").ToDictionary(i => i.GetInstanceID());
        //GameObject.FindGameObjectsWithTag("Player").ToList().ForEach(i => NpcsAlive.Add(i.GetInstanceID(), i));

        //特效池
        AttackFx = new Queue<GameObject>(100);
        InitAttackFx();
        DieFx = new Queue<GameObject>(20);
        InitDieFx();
        BornFx = new Queue<GameObject>(20);
        InitBornFx();

        //載入短暫浮現的特效或物件
        DragonFireBallExplosionFx = (GameObject)Resources.Load("BigExplosion"); //龍龍火球爆炸特效
        TimeStopChain = Instantiate((GameObject)Resources.Load("FX_TimeStopKeep")); //時間暫停
        TimeStopChain.SetActive(false);
        //...
        Statics = GameObject.FindGameObjectsWithTag("Terrain").ToList();
        ChasingNpc = new HashSet<AiState>();

        var meshs = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (var item in meshs)
        {
            item.enabled = false;
        }

        //場景一的東西
        //FirstStage
        //GenUsao(stageOneSpawnPoint.position, 10, 15, GameState.FirstStage);
        StageMonsterMonitor[1] = 30;

        //SecondStage
        GenUsao2(stageTwoSpawnPoint.position, 10, 10, GameState.SecondStage);
        StageMonsterMonitor[2] = 10;

        //ThirdStage
        GenUsaoSword(stageThreeSpawnPoint.position, 10, 10, GameState.ThridStage);
        StageMonsterMonitor[3] = 10;
    }

    private void InitBornFx()
    {
        var fx = (GameObject)Resources.Load("FX_MobBorn");
        for (int i = 0; i < 20; i++)
        {
            var go = Instantiate(fx);
            BornFx.Enqueue(go);
        }
    }

    private void InitDieFx()
    {
        var fx = (GameObject)Resources.Load("FX_NPC_Die");
        for (int i = 0; i < 20; i++)
        {
            var go = Instantiate(fx);
            DieFx.Enqueue(go);
        }
    }

    public static void GenUsaoSword(Vector3 position, int range, int normalNumber, GameState state)
    {
        var usao = (GameObject)Resources.Load("usao_WithSword");

        for (int i = 0; i < normalNumber; i++)
        {
            usao.transform.position = position + new Vector3(UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1), 1, UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1));
            usao.transform.forward = ObjectManager.MainCharacter.position - usao.transform.position;
            var go = Instantiate(usao);
            var npc = go.GetComponent<Npc>();
            StagePools[(int)state].Add(npc);
            npc.Hp = 200;
            npc.gameState = state;
            
            var showMap = go.transform.FindAnyChild<ShowOnMap>("MiniUsao");
            showMap.showStage = (int)state ;
            NpcsAlive.Add(go.GetInstanceID(), go);
        }
    }
    public static void StageOneResurrection()
    {
        var position = stageOneSpawnPoint.position;
        var range = 10;

        for (int i = 0; i < 10; i++)
        {
            var bornFx = BornFx.Dequeue();
            bornFx.transform.position = position + new Vector3(UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1), 1, UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1));

            bornFx.SetActive(true);
            BornFx.Enqueue(bornFx);
        }
        //全體復活
        foreach (var usao in StageDeathPool[1].Values)
        {
            usao.transform.position = position + new Vector3(UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1), 1, UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1));
            var npc = usao.GetComponent<Npc>();
            npc.Hp = npc.MaxHp;
            var ator = usao.GetComponent<Animator>();
            var manager = usao.GetComponent<UsaoManager>();
            manager.StartAiState();
            ator.Play("Fight");
            usao.SetActive(true);
            NpcsAlive.Add(usao.GetInstanceID(), usao);
            StageMonsterMonitor[1]++;

        }
        StageDeathPool[1].Clear();
        return;
    }
    internal static void StageTwoResurrection()
    {
        var position = stageTwoSpawnPoint.position;
        var range = 5;
        if (UnityEngine.Random.value < 0.9995f) return;
        //全體復活
        foreach (var usao in StageDeathPool[2].Values)
        {
            usao.transform.position = position + new Vector3(UnityEngine.Random.Range(1, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1), 0, UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1));
            var npc = usao.GetComponent<Npc>();
            npc.Hp = npc.MaxHp;
            var ator = usao.GetComponent<Animator>();
            var manager = usao.GetComponent<UsaoManager>();
            manager.StartAiState();
            ator.Play("Fight");
            usao.SetActive(true);
            NpcsAlive.Add(usao.GetInstanceID(), usao);
            StageMonsterMonitor[2]++;

        }
        StageDeathPool[2].Clear();
        return;
    }


    internal static void StageThreeResurrection()
    {
        var position = stageThreeSpawnPoint.position;
        var range = 5;
        if (UnityEngine.Random.value < 0.9995f) return;
        //全體復活
        foreach (var usao in StageDeathPool[3].Values)
        {
            usao.transform.position = position + new Vector3(UnityEngine.Random.Range(1, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1), 0, UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1));
            var npc = usao.GetComponent<Npc>();
            npc.Hp = npc.MaxHp;
            var ator = usao.GetComponent<Animator>();
            var manager = usao.GetComponent<UsaoManager>();
            manager.StartAiState();
            ator.Play("Fight");
            usao.SetActive(true);
            NpcsAlive.Add(usao.GetInstanceID(), usao);
            StageMonsterMonitor[3]++;

        }
        StageDeathPool[3].Clear();
        return;
    }
    public static void GenUsao2(Vector3 position, int range, int normalNumber, GameState state)
    {
        var usao = (GameObject)Resources.Load("usao_0321_2");

        for (int i = 0; i < normalNumber; i++)
        {
            usao.transform.position = position + new Vector3(UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1), 1, UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1));
            usao.transform.forward = ObjectManager.MainCharacter.position - usao.transform.position;
            var go = Instantiate(usao);
            var npc = go.GetComponent<Npc>();
            StagePools[(int)state].Add(npc);
            npc.Hp = 150;
            npc.gameState = state;
            var showMap = go.transform.FindAnyChild<ShowOnMap>("MiniUsao");
            showMap.showStage = (int)state;
            NpcsAlive.Add(go.GetInstanceID(), go);
        }
    }

    public static void GenUsao(Vector3 position, float range, int normalNumber, GameState state)
    {
        var usao = (GameObject)Resources.Load("usao_0321");

        for (int i = 0; i < normalNumber; i++)
        {
            usao.transform.position = position + new Vector3(UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1), 1, UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1));
            usao.transform.forward = ObjectManager.MainCharacter.position - usao.transform.position;
            var go = Instantiate(usao);
            var npc = go.GetComponent<Npc>();
            StagePools[(int)state].Add(npc);
            npc.Hp = 50;
            npc.gameState = state;
            
            var showMap = go.transform.FindAnyChild<ShowOnMap>("MiniUsao");
            showMap.showStage = (int)state ;
            NpcsAlive.Add(go.GetInstanceID(), go);
        }
    }
    void InitAttackFx()
    {
        var fx = (GameObject)Resources.Load("CFXR Hit A (Red)");
        for (int i = 0; i < 100; i++)
        {
            var go = Instantiate(fx);
            AttackFx.Enqueue(go);
        }
    }

}
