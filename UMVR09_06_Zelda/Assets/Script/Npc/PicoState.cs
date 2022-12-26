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
    public GameState GameState { get; set; }
    public GameState TargetState { get; set; }
    // Start is called before the first frame update
    public Transform FirstMiniMap;
    public Transform SecondMiniMap;
    public Transform ThirdMiniMap;
    public Transform ForthMiniMap;
    void Start()
    {
        GameState = GameState.ZeroStage;
        TargetState = GameState.FirstStage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LevelOneFinished()
    {
        FirstMiniMap.gameObject.SetActive(false);
        SecondMiniMap.gameObject.SetActive(true);
        TargetState = GameState.SecondStage;
    }
    public void LevelTwoFinished()
    {
        SecondMiniMap.gameObject.SetActive(false);
        ThirdMiniMap.gameObject.SetActive(true);
        TargetState = GameState.ThridStage;
    }
    public void LevelThreeFinished()
    {
        Debug.Log("LevelThreeFinished");
        Debug.Log("LevelThreeFinished");
        Debug.Log("LevelThreeFinished");
        Debug.Log("LevelThreeFinished");
        Debug.Log("LevelThreeFinished");
        ThirdMiniMap.gameObject.SetActive(false);
        ForthMiniMap.gameObject.SetActive(true);
        TargetState = GameState.FourthStage;
    }
}
