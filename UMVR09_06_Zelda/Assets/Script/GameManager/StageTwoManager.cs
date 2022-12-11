using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTwoManager : MonoBehaviour
{
    GameState state = GameState.SecondStage;
    PicoState picoState;
    int mobCount;
    public DragonManager Dragon;
    bool finished => Dragon.Hp <= 0;
    // Start is called before the first frame update
    void Start()
    {
        picoState = ObjectManager.MainCharacter.GetComponent<PicoState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == picoState.gameState)
        {

        }
    }
}
