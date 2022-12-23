using Cinemachine;
using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public Transform PicoTpPlace;
    bool PicoCanTp = true;
    BlackFade1 BlackScreen;

    public int TriggerType;
    public Transform Pico;
    public float distance = 10;
    PicoState picoState;
    int stageOneWave;
    [HideInInspector] public DragonManager Dragon;
    [HideInInspector] public GolemManager Golem;
    [HideInInspector] public SpaceManager Space;
    public PlayableDirector dragonDirector;
    public PlayableDirector spaceDirector;
    public PlayableDirector golemDirector;
    [HideInInspector] public CinemachineVirtualCamera dragonVirtualCamera;
    [HideInInspector] public CinemachineVirtualCamera golemVirtualCamera;
    [HideInInspector] public CinemachineVirtualCamera spaceVirtualCamera;
    bool isNightScene;
    private bool dragonPlay;
    private bool golemStand;
    private bool spacePlay;

    //scene 1 
    LevelHandler levelHandler1;
    LevelHandler levelHandler2;
    LevelHandler levelHandler3;

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
        if (isNightScene)
        {
            levelHandler1 = GameObject.Find("Obelisk_zodiac_low (1)").GetComponent<LevelHandler>();
            levelHandler2 = GameObject.Find("Obelisk_zodiac_low (2)").GetComponent<LevelHandler>();
            levelHandler3 = GameObject.Find("Obelisk_zodiac_low").GetComponent<LevelHandler>();
            Dragon = GameObject.Find("Blue Variant").GetComponent<DragonManager>();
            Golem = GameObject.Find("PBR_Golem (1)").GetComponent<GolemManager>();
            dragonVirtualCamera = GameObject.Find("CM vcam3").GetComponent<CinemachineVirtualCamera>();
            golemVirtualCamera = GameObject.Find("CM vcam4").GetComponent<CinemachineVirtualCamera>();
        }
        else
        {
            BlackScreen = GameObject.Find("BlackScreen").GetComponent<BlackFade1>();
            Space = GameObject.Find("space3").GetComponent<SpaceManager>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        picoState = Pico.GetComponent<PicoState>();
        if (TriggerType == 1)
        {
            stageOneWave = 2;//第一關會有 2 波
        }
        dragonVirtualCamera.Priority = 5;
        dragonVirtualCamera.gameObject.SetActive(false);
        golemVirtualCamera.Priority = 5;
        golemVirtualCamera.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var d = Vector3.Distance(this.transform.position, Pico.position);
        if (d <= distance)
        {
            picoState.gameState = (GameState)TriggerType;
            if (isNightScene)
                ObjectManager.myCamera.stage = TriggerType;
            else
                ObjectManager2.myCamera.stage = TriggerType;
        }

        if (isNightScene)
        {
            switch ((int)picoState.gameState)
            {
                case 1:
                    if (stageOneWave <= 0 && ObjectManager.StageMonsterMonitor[1] <= 0)
                    {
                        levelHandler1.FinishThisLevel();
                        return;
                    }
                    if (stageOneWave > 0 && ObjectManager.StageMonsterMonitor[1] < 15)
                    {
                        ObjectManager.GenUsao2(ObjectManager.stageOneSpawnPoint.position, 10, 5, GameState.FirstStage);
                        ObjectManager.StageMonsterMonitor[1] += 5;
                        ObjectManager.GenUsaoSword(ObjectManager.stageOneSpawnPoint.position, 10, 10, GameState.FirstStage);
                        ObjectManager.StageMonsterMonitor[1] += 10;
                        ObjectManager.StageOneResurrection();
                        stageOneWave--;
                    }
                    return;
                case 2:
                    if (Dragon.Hp <= 0)
                    {
                        levelHandler2.FinishThisLevel();
                        return;
                    }
                    if (!dragonPlay && ObjectManager.StageMonsterMonitor[2] <= 0)
                    {
                        dragonVirtualCamera.gameObject.SetActive(true);
                        dragonPlay = true;
                        dragonVirtualCamera.Priority = 20;
                        dragonDirector.Play();
                    }
                    if (Dragon.Hp > 0 && Dragon.Show && ObjectManager.StageMonsterMonitor[2] < 10)
                    {
                        ObjectManager.StageTwoResurrection();
                    }
                    return;
                case 3:

                    if (Golem.Hp <= 0)
                    {
                        levelHandler3.FinishThisLevel();
                        return;
                    }
                    if (!golemStand && ObjectManager.StageMonsterMonitor[3] <= 0)
                    {
                        golemStand = true;
                        golemVirtualCamera.gameObject.SetActive(true);
                        golemVirtualCamera.Priority = 20;
                        golemDirector.Play();
                    }
                    if (Golem.Stand && Golem.Hp > 0 && ObjectManager.StageMonsterMonitor[3] < 10)
                    {
                        ObjectManager.StageThreeResurrection();
                    }
                    return;
            }
        }
        else
        {
            switch ((int)picoState.gameState)
            {
                case 2:
                    if (!spacePlay && ObjectManager2.StageMonsterMonitor[2] <= 0)
                    {
                        //spaceVirtualCamera.gameObject.SetActive(true);
                        //spaceVirtualCamera.Priority = 20;
                        BlackScreen.FadeOut();
                        if (BlackScreen.newAlpha >= 1)
                        {
                            if (PicoCanTp)
                            {
                                PicoCanTp = false;
                                Pico.transform.position = PicoTpPlace.position;
                            }
                            spaceDirector.Play();
                            BlackScreen.FadeIn();
                            spacePlay = true;
                        }
                        //dragonPlay = true;
                    }
                    return;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
