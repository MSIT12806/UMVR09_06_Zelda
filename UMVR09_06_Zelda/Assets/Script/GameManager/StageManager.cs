using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int TriggerType;
    public Transform Pico;
    PicoState picoState;
    // Start is called before the first frame update
    void Start()
    {
        picoState = Pico.GetComponent<PicoState>();
    }

    // Update is called once per frame
    void Update()
    {
        var d = Vector3.Distance(this.transform.position, Pico.position);
        if (d <= 10) picoState.gameState = (GameState)TriggerType;
    }
}
