using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    ZeroStage,
    FirstStage,
    SecondStage,
    ThridStage,
    FourthStage,

    FinishStage,
    FailStage

}
public class PicoState : MonoBehaviour
{
    public GameState gameState;
    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.ZeroStage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
