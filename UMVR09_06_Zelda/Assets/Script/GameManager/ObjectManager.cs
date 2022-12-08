using Ron;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static Dictionary<int, GameObject> NpcsAlive;//碰撞偵測、攻擊判定用。
    public static Dictionary<int, GameObject> NpcsDead;
    public static List<GameObject> Statics;
    public static Queue<GameObject> AttackFx;
    public static Dictionary<int, NpcHelper> StateManagers = new Dictionary<int, NpcHelper>();
    public static HashSet<AiState> ChasingNpc;
    public static TPSCamera myCamera;
    public static Transform MainCharacter;
    public static Transform MainCharacterHead;
    public static GameObject DragonFireBallExplosionFx;
    public static GameObject TimeStopChain;
    public Transform MyCharacter;
    //    public static List<GameObject> UsaoResources;
    //處理 npc 碰撞、偵測、迴避、群體運動等行為。
    //chase: 檢查目前會攻擊玩家的角色有幾人，並適時切換 npc 狀態為 around or close。
    private void Awake()
    {
        MainCharacter = MyCharacter;
        MainCharacterHead = MainCharacter.FindAnyChild<Transform>("Head");
        AttackFx = new Queue<GameObject>(20);
        InitAttackFx();
        stageOneSpawnPoint = transform.FindAnyChild<Transform>("StageOneSpawnPoint");
        GenUsao(stageOneSpawnPoint.position, 10, 50);
        NpcsAlive = GameObject.FindGameObjectsWithTag("Npc").ToDictionary(i => i.GetInstanceID());
        GameObject.FindGameObjectsWithTag("Player").ToList().ForEach(i => NpcsAlive.Add(i.GetInstanceID(), i));

        //載入短暫浮現的特效或物件
        DragonFireBallExplosionFx = (GameObject)Resources.Load("BigExplosion"); //龍龍火球爆炸特效
        TimeStopChain = Instantiate((GameObject)Resources.Load("FX_TimeStopKeep")); //時間暫停
        TimeStopChain.SetActive(false);
        //...
        Statics = GameObject.FindGameObjectsWithTag("Terrain").ToList();
        ChasingNpc = new HashSet<AiState>();

        //    UsaoResources = new List<GameObject>(300);
        NpcsDead = new Dictionary<int, GameObject>();

        var meshs = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (var item in meshs)
        {
            item.enabled = false;
        }


    }
    Transform stageOneSpawnPoint;
    void GenUsao(Vector3 position, float range, int normalNumber)
    {
        for (int i = 0; i < normalNumber; i++)
        {
            var usao = (GameObject)Resources.Load("usao_0321");
            usao.transform.position = position + new Vector3(UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1), 1, UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1));
            usao.transform.forward = ObjectManager.MainCharacter.position - usao.transform.position;
            var npc = usao.GetComponent<Npc>();
            npc.Hp = 300;
            npc.gameState = GameState.FirstStage;
            Instantiate(usao);
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
