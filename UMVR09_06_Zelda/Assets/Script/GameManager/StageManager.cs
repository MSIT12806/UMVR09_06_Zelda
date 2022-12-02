using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int TriggerType;
    public Transform Pico;
    public float distance = 10;
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
        if (d <= distance)
        {
            picoState.gameState = (GameState)TriggerType;
            ObjectManager.myCamera.stage = TriggerType;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
