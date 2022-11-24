using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectManager :MonoBehaviour
{
    public static List<GameObject> Npcs ;
    public static List<GameObject> Statics ;
    public static HashSet<AiState> ChasingNpc;
    public static Transform MainCharacter;
    public Transform MyCharacter;

    //處理 npc 碰撞、偵測、迴避、群體運動等行為。
    //chase: 檢查目前會攻擊玩家的角色有幾人，並適時切換 npc 狀態為 around or close。
    private void Awake()
    {
        Npcs = GameObject.FindGameObjectsWithTag("Npc").ToList();
        Npcs.AddRange(GameObject.FindGameObjectsWithTag("Player"));

        Statics = GameObject.FindGameObjectsWithTag("Terrain").ToList();
        ChasingNpc = new HashSet<AiState>();

        MainCharacter = MyCharacter;
    }

}
