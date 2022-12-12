using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ron;
using System;
using UnityEngine.UI;
using Microsoft.Cci;
using static UnityEngine.Rendering.DebugUI;

public class UiManager : MonoBehaviour
{
    // Start is called before the first frame update
    Transform MainCharacterHp;
    Transform GreatEnemyState;
    Transform StrongholdState;
    Transform WeakUi;
    Transform PowerOneKey;
    Image PowerOne;
    Transform PowerTwoKey;
    Image PowerTwo;
    PicoState picoState;
    float currentHp;
    float currentPower;
    Npc mainCharacter;
    string heartPath = "Heart";
    List<GameObject> heartList = new List<GameObject>();
    GameObject heart;
    TPSCamera myCamera;
    GameObject lastHeart;

    float OneHeartHp = 50;

    private Transform ItemUI;
    private float ItemCD;
    private float currentItemCD;

    public Transform[] WeakableMonsters;
    public Transform[] WeakPoints;
    public RectTransform WeakImg;
    public Image WeakFull;
    public Image WeakCrack;
    void Start()
    {
        MainCharacterHp = transform.FindAnyChild<Transform>("MainCharacterHP");
        GreatEnemyState = transform.FindAnyChild<Transform>("GreatEnemyState");
        StrongholdState = transform.FindAnyChild<Transform>("StrongholdState");
        WeakUi = transform.FindAnyChild<Transform>("WeakUi");
        PowerOne = transform.FindAnyChild<Transform>("Power").FindAnyChild<Image>("PowerFull");
        PowerOneKey = transform.FindAnyChild<Transform>("Power").FindAnyChild<Transform>("Key");
        PowerTwo = transform.FindAnyChild<Transform>("Power (1)").FindAnyChild<Image>("PowerFull");
        PowerTwoKey = transform.FindAnyChild<Transform>("Power (1)").FindAnyChild<Transform>("Key");
        picoState = ObjectManager.MainCharacter.GetComponent<PicoState>();
        myCamera = ObjectManager.myCamera;//可以順便拿怪
        ItemUI = transform.FindAnyChild<Transform>("SiKaStone");
        mainCharacter = ObjectManager.MainCharacter.GetComponent<Npc>();
        heart = (GameObject)Resources.Load(heartPath);
        currentHp = PicoManager.Hp;
        ItemCD = mainCharacter.GetComponent<Throw>().coldTime;
        ItemUI.FindAnyChild<Image>("CanLock").fillAmount = 1;
        InitPicoHp();
    }

    void SetWeakPoint(NpcHelper nh)
    {
        var Value = nh.WeakPoint;
        var MaxValue = nh.MaxWeakPoint;
        if (Value >= MaxValue / 2)
        {
            WeakFull.fillAmount = (Value - MaxValue / 2) / (MaxValue / 2);
            WeakCrack.fillAmount = 1;
        }
        else if (Value < MaxValue / 2)
        {
            WeakFull.fillAmount = 0;
            WeakCrack.fillAmount = Value / (MaxValue / 2);
        }

        //放完終結技、重新填充的議題？
    }

    bool weakShow = true;
    // Update is called once per frame
    void Update()
    {
        SetHpBar();
        SetFeverBar();
        var ScriptThrow = mainCharacter.GetComponent<Throw>();
        currentItemCD = ScriptThrow.timer;

        float appleFill = ScriptThrow.appleCount/6;
        ItemUI.FindAnyChild<Image>("CanEatApple").fillAmount = appleFill;//蘋果ui控制
        
        if (currentItemCD != 0)
        {
            SikaStoneCD();
        }

        //據點

        //大怪--注視
        if (myCamera.cameraState == "Stare")
        {
            GreatEnemyState.gameObject.SetActive(true);
            StrongholdState.gameObject.SetActive(false);
            RefreshGreatEnemyState();
        }
        else
        {
            GreatEnemyState.gameObject.SetActive(false);
            StrongholdState.gameObject.SetActive(false);
        }
        //大怪--弱點槽 //希卡指示器？
        for (int i = 0; i < WeakableMonsters.Length; i++)
        {
            var item = WeakableMonsters[i];
            if (item == null) break;
            var nh = ObjectManager.StateManagers[item.gameObject.GetInstanceID()];
            if (nh.Dizzy)
            {
                SetWeakPoint(nh);
                Vector2 v = Camera.main.WorldToScreenPoint(WeakPoints[i].position);
                WeakImg.position = v;
                if (weakShow == true) return;

                WeakImg.gameObject.SetActive(true);
                weakShow = true;
                return;
            }
            else
            {
                if (weakShow == false) continue;

                WeakImg.gameObject.SetActive(false);
                weakShow = false;


            }
        }


    }

    private void RefreshGreatEnemyState()
    {
        var hp = GreatEnemyState.transform.FindAnyChild<Image>("GreatEnemyHpBarFull");
        var hpInfo = myCamera.m_StareTarget[(int)picoState.gameState].GetComponent<Npc>();
        hp.fillAmount = hpInfo.Hp / hpInfo.MaxHp;
    }

    void SetHpBar()
    {

        if (PicoManager.Hp != currentHp)
        {
            currentHp = PicoManager.Hp;
            FillHeart();

        }
    }

    private void InitPicoHp()
    {
        var nowHp = currentHp;
        var heartCount = (int)Math.Ceiling(PicoManager.MaxHp / OneHeartHp);
        for (int i = 0; i < heartCount; i++)
        {
            if (nowHp <= 0) break;

            var h = Instantiate(heart);
            h.transform.SetParent(MainCharacterHp);
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
        var heartNum = nowHp / OneHeartHp;
        for (int i = 0; i < heartNum; i++)
        {
            var hf = heartList[i].transform.FindAnyChild<Image>("HeartFull");
            if (nowHp > OneHeartHp)
            {
                hf.fillAmount = 1;
                nowHp -= OneHeartHp;
            }
            else
            {
                hf.fillAmount = nowHp / OneHeartHp;
            }
        }
    }
    void InitFeverBar()
    {


    }
    void SetFeverBar()
    {
        if (PicoManager.Power == currentPower) return;

        currentPower = PicoManager.Power;
        if (PicoManager.Power == 200)
        {
            PowerOne.fillAmount = 1;
            PowerTwo.fillAmount = 1;
            PowerOneKey.gameObject.SetActive(false);
            PowerTwoKey.gameObject.SetActive(true);
        }
        else if (PicoManager.Power >= 100)
        {
            PowerOne.fillAmount = 1;
            PowerTwo.fillAmount = (PicoManager.Power - 100) / 100;
            PowerOneKey.gameObject.SetActive(true);
            PowerTwoKey.gameObject.SetActive(false);
        }
        else
        {
            PowerTwo.fillAmount = 0;
            PowerOne.fillAmount = (PicoManager.Power) / 100;
            PowerOneKey.gameObject.SetActive(false);
            PowerTwoKey.gameObject.SetActive(false);
        }
    }

    private void SikaStoneCD()
    {
        var sCD = currentItemCD / ItemCD; 
        ItemUI.FindAnyChild<Image>("CanLock").fillAmount = sCD;
        ItemUI.FindAnyChild<Image>("CanIce").fillAmount = sCD;
        ItemUI.FindAnyChild<Image>("CanBomb").fillAmount = sCD;
        // 現在的CD時間/總CD時間 = 道具中cando 的fill amount
    }
    
}
