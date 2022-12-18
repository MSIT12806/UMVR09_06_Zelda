using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ron;
using System;
using UnityEngine.UI;
using Microsoft.Cci;
using static UnityEngine.Rendering.DebugUI;
using TMPro;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static UiManager singleton;
    public  BlackFade BlackMask;
    Transform MainCharacterHp;
    Transform GreatEnemyState;
    Transform StrongholdState;
    Transform WeakUi;
    Transform PowerOneKey;
    Transform PowerOneLight;
    Image PowerOne;
    Transform PowerTwoKey;
    Transform PowerTwoLight;
    [HideInInspector] public Transform SikaTools;
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
    RectTransform ImgToShow;
    public Image WeakFull;
    public Image WeakCrack;

    bool isNightScene;
    private void Awake()
    {
        singleton = this;

        var currentScene = SceneManager.GetActiveScene();
        var currentSceneName = currentScene.name;
        if (currentSceneName == "NightScene")
        {
            isNightScene = true;
        }
        else
        {
            isNightScene = false;
        }
    }

    void Start()
    {

        MainCharacterHp = transform.FindAnyChild<Transform>("MainCharacterHP");
        GreatEnemyState = transform.FindAnyChild<Transform>("GreatEnemyState");
        StrongholdState = transform.FindAnyChild<Transform>("StrongholdState");
        WeakUi = transform.FindAnyChild<Transform>("Weakness");
        PowerOne = transform.FindAnyChild<Transform>("Power").FindAnyChild<Image>("PowerFull");
        PowerOneKey = transform.FindAnyChild<Transform>("Power").FindAnyChild<Transform>("Key");
        PowerOneLight = transform.FindAnyChild<Transform>("Power").FindAnyChild<Transform>("PowerLight");
        PowerTwo = transform.FindAnyChild<Transform>("Power (1)").FindAnyChild<Image>("PowerFull");
        PowerTwoKey = transform.FindAnyChild<Transform>("Power (1)").FindAnyChild<Transform>("Key");
        PowerTwoLight = transform.FindAnyChild<Transform>("Power (1)").FindAnyChild<Transform>("PowerLight");
        SikaTools = transform.FindAnyChild<Transform>("ItemTips");
        if (isNightScene)
        {
            picoState = ObjectManager.MainCharacter.GetComponent<PicoState>();
            myCamera = ObjectManager.myCamera;//可以順便拿怪
            mainCharacter = ObjectManager.MainCharacter.GetComponent<Npc>();
        }
        else
        {
            picoState = ObjectManager2.MainCharacter.GetComponent<PicoState>();
            myCamera = ObjectManager2.myCamera;//可以順便拿怪
            mainCharacter = ObjectManager2.MainCharacter.GetComponent<Npc>();
        }
        ItemUI = transform.FindAnyChild<Transform>("SiKaStone");
        heart = (GameObject)Resources.Load(heartPath);
        currentHp = PicoManager.Hp;
        ItemCD = mainCharacter.GetComponent<Throw>().coldTime;
        ItemUI.FindAnyChild<Image>("CanLock").fillAmount = 1;
        InitPicoHp();
        currentPower = float.MinValue;
    }
    public void Fail()
    {
        transform.FindAnyChild<Transform>("Fail").gameObject.SetActive(true);
        Invoke("FadeOut", 3);
    }

    public void FadeOut()
    {
        BlackMask.FadeOut();
    }
    public void Success()
    {
        transform.FindAnyChild<Transform>("Success").gameObject.SetActive(true);
    }
    public void ShowSikaTip(string sikaType)
    {
        //初始化        
        var ItemLockTips = SikaTools.FindAnyChild<RectTransform>("TimeStopTipsRing");
        ItemLockTips.gameObject.SetActive(false);
        var ItemBombTips = SikaTools.FindAnyChild<RectTransform>("BombTipsRing");
        ItemBombTips.gameObject.SetActive(false);
        var ItemIceTips = SikaTools.FindAnyChild<RectTransform>("IceTipsRing");
        ItemIceTips.gameObject.SetActive(false);

        SikaTools.gameObject.SetActive(true);


        switch (sikaType)
        {
            case "ItemLockTips":
                ImgToShow = ItemLockTips;
                break;
            case "ItemBombTips":
                ImgToShow = ItemBombTips;
                break;
            case "ItemIceTips":
                ImgToShow = ItemIceTips;
                break;
            default:
                ImgToShow = WeakUi.GetComponent<RectTransform>();
                SikaTools.gameObject.SetActive(false);
                break;
        }

        ImgToShow.gameObject.SetActive(true);
        if (ImgToShow.TryGetComponent<ParticleSystem>(out var p))
        {
            p.Play();
        }
    }

    public void HideTip()
    {
        if (ImgToShow == null) return;
        ImgToShow.gameObject.SetActive(false);
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

    bool tipShow = true;
    // Update is called once per frame
    void Update()
    {
        SetHpBar();
        SetFeverBar();
        var ScriptThrow = mainCharacter.GetComponent<Throw>();
        currentItemCD = ScriptThrow.timer;

        float appleFill = PicoManager.AppleCount / 6;
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
            if ((int)picoState.gameState - 2 != 0 && (int)picoState.gameState - 2 != 1) return;
            var item = WeakableMonsters[i];
            if (item == null) break;
            if (item.name != WeakableMonsters[(int)picoState.gameState - 2].name) continue;
            var nh = ObjectManager.StateManagers[item.gameObject.GetInstanceID()];
            if (nh.Dizzy)
            {
                ShowSikaTip("");
                SetWeakPoint(nh);
                if (tipShow == true) continue;
                ImgToShow.gameObject.SetActive(true);
                tipShow = true;
            }
            else
            {
                if (tipShow == false) continue;

                tipShow = false;
                ImgToShow.gameObject.SetActive(false);
            }
        }
        ImageFollow((int)picoState.gameState - 2);

    }
    void ImageFollow(int mosterType)
    {
        if (ImgToShow == null) return;
        if (mosterType == 0 || mosterType == 1)
        {
            Vector2 v = Camera.main.WorldToScreenPoint(WeakPoints[mosterType].position);
            ImgToShow.position = v;
        }
    }
    private void RefreshGreatEnemyState()
    {
        var a = GreatEnemyState.transform.FindAnyChild<Transform>("GreatEnemyBar");
        var b = a.FindAnyChild<Transform>("GreatEnemyName");
        var nameUi = b.GetComponent<TextMeshProUGUI>();
        var hp = GreatEnemyState.transform.FindAnyChild<Image>("GreatEnemyHpBarFull");
        var hpInfo = myCamera.m_StareTarget[(int)picoState.gameState].GetComponent<Npc>();
        hp.fillAmount = hpInfo.Hp / hpInfo.MaxHp;
        if (isNightScene)
            nameUi.text = ObjectManager.StateManagers[myCamera.m_StareTarget[(int)picoState.gameState].gameObject.GetInstanceID()].Name;
        else
            nameUi.text = ObjectManager2.StateManagers[myCamera.m_StareTarget[(int)picoState.gameState].gameObject.GetInstanceID()].Name;
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
            PowerOneLight.gameObject.SetActive(false);
            PowerTwoKey.gameObject.SetActive(true);
            PowerTwoLight.gameObject.SetActive(true);
        }
        else if (PicoManager.Power >= 100)
        {
            PowerOne.fillAmount = 1;
            PowerTwo.fillAmount = (PicoManager.Power - 100) / 100;
            PowerOneKey.gameObject.SetActive(true);
            PowerOneLight.gameObject.SetActive(true);
            PowerTwoKey.gameObject.SetActive(false);
            PowerTwoLight.gameObject.SetActive(false);
        }
        else
        {
            PowerTwo.fillAmount = 0;
            PowerOne.fillAmount = (PicoManager.Power) / 100;
            PowerOneKey.gameObject.SetActive(false);
            PowerOneLight.gameObject.SetActive(false);
            PowerTwoKey.gameObject.SetActive(false);
            PowerTwoLight.gameObject.SetActive(false);
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
