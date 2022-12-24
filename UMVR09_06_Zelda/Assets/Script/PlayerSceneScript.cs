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
    void LateUpdate()
    {
        FadeIn();
        if (ChangeScene)
        {
            StartCoroutine(LoadYourAsyncScene());
        }
    }
    IEnumerator LoadYourAsyncScene()
    {

        PicoManager.Hp = PicoManager.MaxHp;
        PicoManager.Power = 0;
        PicoManager.AppleCount = PicoManager.MaxApple;

        // 等到異步場景完全加載
        while (Alpha < 1)
        {
            Alpha += 0.05f;
            BlackScreen.color = new Color(BlackScreen.color.r, BlackScreen.color.g, BlackScreen.color.b, Alpha);
            yield return null;
        }

        SceneManager.LoadScene(1);
        // Wait until the asynchronous scene fully loads
        //while ()
        //{
        //    yield return null;
        //}
    }
    public void FadeIn()
    {
        canvasGroup.alpha += 0.01f;
    }
    public void StartButtonClick()
    {
        StartButton();
    }
    void StartButton()
    {
        if (ChangeScene) return;
        //yield return new WaitForSeconds(1.5f);
        ChangeScene = true;
        BlackScreen.gameObject.SetActive(true);
    }
}
