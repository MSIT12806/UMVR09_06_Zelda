using Cinemachine;
using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public int TriggerType;
    public Transform Pico;
    public float distance = 10;
    PicoState picoState;
    int stageOneWave;
    public DragonManager Dragon;
    public GolemManager Golem;
    public PlayableDirector director;
    public CinemachineVirtualCamera dragonVirtualCamera;
    public CinemachineVirtualCamera golemVirtualCamera;
    bool isNightScene;
    private bool dragonPlay;
    private bool golemStand;

    private void Awake()
    {
        var currentScene = SceneManager.GetActiveScene();
        var currentSceneName = currentScene.name;
        if (currentSceneName == "NightScene")
        {
            isNightScene = true;
        }
        else
        {
            isNightScene = false;
        }
        if (!isNightScene) return;
        Dragon = GameObject.Find("Blue Variant").GetComponent<DragonManager>();
        Golem = GameObject.Find("PBR_Golem (1)").GetComponent<GolemManager>();
        dragonVirtualCamera = GameObject.Find("CM vcam3").GetComponent<CinemachineVirtualCamera>();
        golemVirtualCamera = GameObject.Find("CM vcam4").GetComponent<CinemachineVirtualCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        picoState = Pico.GetComponent<PicoState>();
        if (TriggerType == 1)
        {
            stageOneWave = 4;//第一關會有四波
        }


    }

    // Update is called once per frame
    void Update()
    {
        var d = Vector3.Distance(this.transform.position, Pico.position);
        if (d <= distance)
        {
            picoState.gameState = (GameState)TriggerType;
            Debug.Log(picoState.gameState);
            if (isNightScene)
                ObjectManager.myCamera.stage = TriggerType;
            else
                ObjectManager2.myCamera.stage = TriggerType;
        }

        if (isNightScene)
        {
            switch (TriggerType)
            {
                case 1:
                    if (stageOneWave > 0 && ObjectManager.StageMonsterMonitor[1] < 10)
                    {
                        ObjectManager.StageOneResurrection();
                        stageOneWave--;
                    }
                    return;
                case 2:
                    if (!dragonPlay && ObjectManager.StageMonsterMonitor[2] <= 0)
                    {
                        dragonPlay = true;
                        dragonVirtualCamera.Priority = 20;
                        director.Play();
                    }
                    if (Dragon.Hp > 0 && Dragon.Show && ObjectManager.StageMonsterMonitor[2] < 10)
                    {
                        ObjectManager.StageTwoResurrection();
                    }
                    return;
                case 3:
                    if (!golemStand && ObjectManager.StageMonsterMonitor[3] <= 0)
                    {
                        golemStand = true;
                        Golem.Stand = true;
                        var a = Golem.GetComponent<Animator>();
                        a.SetFloat("StandSpeed", 1);
                        var r = Golem.transform.FindAnyChild<Transform>("RockGolemMesh").GetComponent<Renderer>();
                        var m = r.materials[0];
                        m.SetColor("_Emissive_Color", new Color(0.737f, 0.737f, 0.737f, 1));
                    }
                    if (golemStand && Golem.Hp > 0 && ObjectManager.StageMonsterMonitor[3] < 10)
                    {
                        ObjectManager.StageThreeResurrection();
                    }
                    return;
            }
        }
        else
        {

        }
    }
    public void StageTwoShowFinished()
    {
        dragonVirtualCamera.Priority = 5;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
