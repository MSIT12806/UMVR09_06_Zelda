using Ron;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static List<GameObject> Npcs;
    public static List<GameObject> Statics;
    public static HashSet<AiState> ChasingNpc;
    public static Transform MainCharacter;
    public Transform MyCharacter;
    //    public static List<GameObject> UsaoResources;
    //處理 npc 碰撞、偵測、迴避、群體運動等行為。
    //chase: 檢查目前會攻擊玩家的角色有幾人，並適時切換 npc 狀態為 around or close。
    private void Awake()
    {
        stageOneSpawnPoint = transform.FindAnyChild<Transform>("StageOneSpawnPoint");
        print(stageOneSpawnPoint.position);
        GenUsao(stageOneSpawnPoint.position, 10, 50);
        Npcs = GameObject.FindGameObjectsWithTag("Npc").ToList();
        Npcs.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        Statics = GameObject.FindGameObjectsWithTag("Terrain").ToList();
        ChasingNpc = new HashSet<AiState>();

        MainCharacter = MyCharacter;
        //    UsaoResources = new List<GameObject>(300);


    }
    Transform stageOneSpawnPoint;
    public void GenUsao(Vector3 position, float range, int normalNumber)
    {
        for (int i = 0; i < normalNumber; i++)
        {
            var usao = (GameObject)Resources.Load("usao_0321");
            usao.transform.position = position + new Vector3(UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1), 1, UnityEngine.Random.Range(3, range) * (UnityEngine.Random.Range(0, 2) * 2 - 1));
            var npc = usao.GetComponent<Npc>();
            npc.Hp = 50;
            Instantiate(usao);
        }
    }
}
