using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowOnMap : MonoBehaviour
{
    public int showStage;
    PicoState gameState;
    private bool isNight;
    MeshRenderer mesh;
    // Start is called before the first frame update
    void Start()
    {
        var currentScene = SceneManager.GetActiveScene();
        var currentSceneName = currentScene.name;
        if (currentSceneName == "NightScene")
        {
            gameState = ObjectManager.MainCharacter.GetComponent<PicoState>();
            mesh = GetComponent<MeshRenderer>();
            isNight = true;
        }
            
    }

    // Update is called once per frame
    void Update()
    {
        if (!isNight) return;
        if((int)gameState.GameState == showStage)
        {
            mesh.enabled = true;
        }
        else
        {
            mesh.enabled = false;
        }
    }
}
