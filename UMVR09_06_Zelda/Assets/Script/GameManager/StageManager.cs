using Cinemachine;
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
    public CinemachineVirtualCamera camera;
    bool isNightScene;
    private bool play;

    // Start is called before the first frame update
    void Start()
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
                    if (!play && ObjectManager.StageMonsterMonitor[2] <= 0)
                    {
                        play = true;
                        camera.Priority = 20;
                        director.Play();
                    }
                    if (Dragon.Hp > 0 && Dragon.Show && ObjectManager.StageMonsterMonitor[2] < 10)
                    {
                        ObjectManager.StageTwoResurrection();
                    }
                    return;
                case 3:
                    if (Golem.Hp > 0 && ObjectManager.StageMonsterMonitor[3] < 10)
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
        camera.Priority = 5;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
