using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSceneScript : MonoBehaviour
{
    // Start is called before the first frame update
    public CanvasGroup canvasGroup;
    public Image BlackScreen;
    bool ChangeScene = false;
    float Alpha = 0;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FadeIn();

        if (ChangeScene)
        {
            Alpha += 0.05f;
            BlackScreen.color = new Color(BlackScreen.color.r, BlackScreen.color.g, BlackScreen.color.b, Alpha);

            if (Alpha >= 1)
            {
                PicoManager.Hp = PicoManager.MaxHp;
                PicoManager.Power = 0;
                Invoke("StartGame", 0.1f);
            }
        }
    }
    void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void FadeIn()
    {
        canvasGroup.alpha += 0.01f;
    }
    public void StartButtonClick()
    {
        StartCoroutine(StartButton());
    }
    IEnumerator StartButton()
    {
        //yield return new WaitForSeconds(1.5f);
        ChangeScene = true;
        BlackScreen.gameObject.SetActive(true);
        yield return 0;
    }
}
