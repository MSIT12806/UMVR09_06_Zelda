using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackFade : MonoBehaviour
{
    public Image fadeImage;
    public float time;
    private float newAlpha;
    public bool IsFadeIn = true;
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
                newAlpha = fadeImage.color.a - 0.01f;
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, newAlpha);
            }
        }
        else
        {
            if (newAlpha < 1)
            {
                time -= Time.deltaTime;
                newAlpha = fadeImage.color.a + 0.01f;
                fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, newAlpha);
            }
        }
    }

    public void FadeIn()
    {
        IsFadeIn = true;
    }
    public void FadeOut()
    {
        IsFadeIn = false;
    }
    public void Open()
    {
        fadeImage.gameObject.SetActive(true);
    }
}
