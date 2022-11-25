using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ron;
using System;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform HpUi;
    float currentHp;
    float currentFever;
    Npc mainCharacter;
    string heartPath = "Heart";
    List<GameObject> heartList = new List<GameObject>();
    GameObject heart;

    GameObject lastHeart;
    void Start()
    {
        HpUi = transform.FindAnyChild<Transform>("MainCharacterHP");
        mainCharacter = ObjectManager.MainCharacter.GetComponent<Npc>();
        heart = (GameObject)Resources.Load(heartPath);
        currentHp = mainCharacter.Hp;
        InitHp();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCharacter.Hp != currentHp)
        {
            currentHp = mainCharacter.Hp;
            SetHpBar();
        }
    }

    void SetHpBar()
    {

        FillHeart();

    }

    private void InitHp()
    {
        var nowHp = currentHp;
        var heartCount = (int)Math.Ceiling(currentHp / 100);
        for (int i = 0; i < heartCount; i++)
        {
            if (nowHp <= 0) break;

            var h = Instantiate(heart);
            h.transform.SetParent(HpUi);
            heartList.Add(h);
            lastHeart = h;
        }
    }

    private void FillHeart()
    {
        var nowHp = currentHp;
        foreach (var item in heartList)
        {
            var hf = item.transform.FindAnyChild<Image>("HeartFull");
            hf.fillAmount = 0;
        }
        for (int i = 0; i < nowHp/100; i++)
        {
            var hf = heartList[i].transform.FindAnyChild<Image>("HeartFull");
            if (nowHp > 100)
            {
                hf.fillAmount = 1;
                nowHp -= 100;
            }
            else
            {
                hf.fillAmount = nowHp / 100;
            }
        }
    }

    void SetFeverBar()
    {

    }
}
