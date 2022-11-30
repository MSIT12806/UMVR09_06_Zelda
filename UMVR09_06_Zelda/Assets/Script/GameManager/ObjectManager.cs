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
    public static Dictionary<int, NpcHelper> StateManagers = new Dictionary<int, NpcHelper>();
    public static HashSet<AiState> ChasingNpc;
    public static Transform MainCharacter;
    public Transform MyCharacter;
    //    public static List<GameObject> UsaoResources;
    //處理 npc 碰撞、偵測、迴避、群體運動等行為。
    //chase: 檢查目前會攻擊玩家的角色有幾人，並適時切換 npc 狀態為 around or close。
    Transform stageOneSpawnPoint;
    Transform stageTwoSpawnPoint;
    Transform stageThreeSpawnPoint;
    Transform stageFourSpawnPoint;
    private void Awake()
    {
        stageOneSpawnPoint = transform.FindAnyChild<Transform>("StageOneSpawnPoint");
        stageTwoSpawnPoint = transform.FindAnyChild<Transform>("StageTwoSpawnPoint");
        stageThreeSpawnPoint = transform.FindAnyChild<Transform>("StageThreeSpawnPoint");
        stageFourSpawnPoint = transform.FindAnyChild<Transform>("StageFourSpawnPoint");
        
        NpcsAlive = GameObject.FindGameObjectsWithTag("Npc").ToDictionary(i => i.GetInstanceID());
        GameObject.FindGameObjectsWithTag("Player").ToList().ForEach(i => NpcsAlive.Add(i.GetInstanceID(), i));

        Statics = GameObject.FindGameObjectsWithTag("Terrain").ToList();
        ChasingNpc = new HashSet<AiState>();

        MainCharacter = MyCharacter;
        //    UsaoResources = new List<GameObject>(300);
        NpcsDead = new Dictionary<int, GameObject>();

        Stage(1);

    }
    public void Stage(int stage)
    {
        switch (stage)
        {
            case 1:
                GenUsao(stageOneSpawnPoint.position, 10, 50, stage);
                return;
            case 2:
                GenUsao(stageTwoSpawnPoint.position, 10, 50, stage);
                return;
            default:
                return;
        }
    }
    public void GenUsao(Vector3 position, float range, int normalNumber, int stage)
    {
        for (int i = 0; i < normalNumber; i++)
        {
            var usao = (GameObject)Resources.Load("usao_0321");
            usao.transform.position = position + new Vector3(UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1), 1, UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1));
            var npc = usao.GetComponent<Npc>();
            npc.Hp = 50;
            npc.gameState = (GameState)stage;
            Instantiate(usao);
        }
    }
}
