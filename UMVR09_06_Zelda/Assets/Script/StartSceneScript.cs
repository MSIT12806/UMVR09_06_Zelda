using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneScript : MonoBehaviour
{
    public CanvasGroup a;
    public GameObject startUI;
    bool FadeOut = false;
    public AudioSource ButtonSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (FadeOut)
        {
            a.alpha -= 0.05f;
        }
        if(a.alpha <= 0)
        {
            StartCoroutine(ChangeUI());
        }
    }
    IEnumerator ChangeUI()
    {
        yield return new WaitForSeconds(0.1f);
        startUI.SetActive(true);
        this.gameObject.SetActive(false);
    }
    public void NewGameButtonClick()
    {
        StartCoroutine(NewGame());
    }
    IEnumerator NewGame()
    {
        FadeOut = true;
        ButtonSound.Play();
        yield return new WaitForSeconds(1f);
        //Application.LoadLevel(1);
    }
}
