using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackFade1 : MonoBehaviour
{
    public Image fadeImage;
    public float time;
    public float newAlpha;
    public bool IsFadeIn = true;

    float FadeInSpeed = 0.01f;
    float FadeOutSpeed = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        Open();
        newAlpha = fadeImage.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFadeIn)
        {
            if (newAlpha > 0)
            {
                time += Time.deltaTime;
                newAlpha = fadeImage.color.a - FadeInSpeed;
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, newAlpha);
            }
        }
        else
        {
            if (newAlpha < 1)
            {
                time -= Time.deltaTime;
                newAlpha = fadeImage.color.a + FadeOutSpeed;
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, newAlpha);
            }
        }
        Debug.Log(newAlpha);
    }

    public void FadeIn(float fadeSpeed)
    {
        FadeInSpeed = fadeSpeed;
        IsFadeIn = true;
    }
    public void FadeOut(float fadeSpeed)
    {
        FadeOutSpeed = fadeSpeed;
        IsFadeIn = false;
    }
    public void Open()
    {
        fadeImage.gameObject.SetActive(true);
    }
}
