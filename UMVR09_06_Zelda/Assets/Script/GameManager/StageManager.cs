using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int TriggerType;
    public Transform Pico;
    public float distance = 10;
    PicoState picoState;
    int wave;
    // Start is called before the first frame update
    void Start()
    {
        picoState = Pico.GetComponent<PicoState>();
        if (TriggerType == 1)
        {
            wave = 4;//第一關會有四波
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
                if (wave > 0 && ObjectManager.StageMonsterMonitor[1] < 10)
                {
                    ObjectManager.Resurrection(1);
                    wave--;
                }
                return;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
