using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int TriggerType;
    public Transform Pico;
    public float distance = 10;
    PicoState picoState;
    int stageOneWave;
    public DragonManager Dragon;
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
            ObjectManager.myCamera.stage = TriggerType;
        }

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
                if (ObjectManager.StageMonsterMonitor[2] < 10)
                {
                    ObjectManager.StageTwoResurrection();
                }
                return;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
