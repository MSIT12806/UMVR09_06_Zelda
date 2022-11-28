using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    private enum Item 
    {
        Apple,
        TimeStop,
        Ice,
        Bomb,
    }

    private Vector3 start_pos;
    private Vector3 current_pos;
    private Vector3 start_vel;
    private Vector3 vel;
    private Vector3 next_vel;
    private Vector3 resistance;
    private Vector3 face;

    private bool CanThrow = true;
    private bool isThrowing = false;
    private Item useItem;

    private Transform ThrowItem;
    public Transform RightHandThrow_pos;
    public GameObject Sword;
    public GameObject SwordEffect;
    private Vector3 itemEffect_pos;
    public GameObject ItemEffect_obj;
    private float Speed =0.25f;
    private float vertical =0.2f;
    private Vector3 Gravity = new Vector3(0,-1,0);

    Animator animator;
    public float coldTime = 10.0f;
    public float timer = 0.0f;
    private bool isStartTime = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanThrow==true) 
        {
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha4))
            {
                GetThrowKeyIn();

                switch (useItem)
                {
                    case Item.TimeStop:
                        UseTimeStop();
                        break;
                    case Item.Ice:
                        UseIce();
                        break;
                    case Item.Bomb:
                        UseBomb();
                        break;
                }
            }
        }
        if (isStartTime) 
        {
            OnThrow();
            CDTimer();
        }
        
        if (ItemEffect_obj != null)
        {
            if(useItem == Item.Ice) DestroyItem(3.5f, "CFXR3 Hit Ice B (Air)");
        }

    }

    void GetThrowKeyIn()  //按鍵切換enum
    {
        if (Input.GetKeyDown(KeyCode.Alpha2)) useItem = Item.TimeStop;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) useItem = Item.Ice;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) useItem = Item.Bomb;
    }

    void UseTimeStop() //使用時停
    {
        isStartTime = true;
        Speed = 0.25f;
        vertical = 0.15f;
        animator.SetTrigger("ThrowTimeStop");
        CanThrow = false;
    }

    void UseIce() //使用冰
    {
        isStartTime = true;
        Speed = 0.25f;
        vertical = 0.0f;
        animator.SetTrigger("ThrowIce");
        CanThrow = false;
    }

    void UseBomb() //使用炸彈
    {
        isStartTime = true;
        Speed = 0.25f;
        vertical = 0.15f;
        animator.SetTrigger("ThrowBomb");
        CanThrow = false;
    }

    public void CDTimer() //計時器
    {
        timer += Time.deltaTime;
        if (timer < coldTime)
        {
            CanThrow = false;
        }
        else if (timer >= coldTime)
        {
            CanThrow = true;
            timer = 0;
            isStartTime = false;
        }
    }

    public void SwordFalse() //讓劍消失（動作事件）
    {
        SwordEffect.GetComponent<ParticleSystem>().Play();
        Sword.SetActive(false);
    }
    public void SwordTrue() //讓劍出現（動作事件）
    {
        Sword.SetActive(true);
        SwordEffect.GetComponent<ParticleSystem>().Play();
    }

    public void GetBomb()  //取得炸彈（動作事件）
    {
        var IK = this.GetComponent<IKController>();
        IK.Weight_Up = 0.0f;

        Object o = Resources.Load("TranslucentCrystal_Bomb");
        GameObject go = Instantiate((GameObject)o);
        go.transform.SetParent(RightHandThrow_pos.transform);
        go.transform.position = RightHandThrow_pos.position;
        ThrowItem = go.transform;
    }

    public void GetIce() //取得冰（動作事件）
    {
        var IK = this.GetComponent<IKController>();
        IK.Weight_Up = 0.0f;

        Object o = Resources.Load("TranslucentCrystal_Ice");
        GameObject go = Instantiate((GameObject)o);
        go.transform.SetParent(RightHandThrow_pos.transform);
        go.transform.position = RightHandThrow_pos.position;
        ThrowItem = go.transform;
    }

    public void GetTimeStop()  //取得時停（動作事件）
    {
        var IK = this.GetComponent<IKController>();
        IK.Weight_Up = 0.0f;

        Object o = Resources.Load("TranslucentCrystal_TimeStop");
        GameObject go = Instantiate((GameObject)o);
        go.transform.SetParent(RightHandThrow_pos.transform);
        go.transform.position = RightHandThrow_pos.position;
        ThrowItem = go.transform;
    }

    public void StartThrow() //丟出時物件位置初始化（動作事件）
    {
        ThrowItem.transform.parent = null;

        face = transform.forward;

        //初始位置 = 物件生成位置
        start_pos = ThrowItem.position;
        //當前位置 = 初始位置
        current_pos = start_pos;

        //初始速度 = 人物面向 * 速率
        start_vel = face * Speed;
        start_vel.y += vertical;
        //當前速度 = 初始速度
        vel = start_vel;

        //Debug.Log(face);

        //阻力 = - 初始速度 * 0.X
        resistance = -(vel) * 0.05f;

        isThrowing = true;
    }

    public void OnThrow()  //每幀物件運動
    {
        if (isThrowing)
        {
            //下一位置 = 當前位置 + 當前速度
            //當前位置 = 下一位置
            current_pos = ThrowItem.position + vel;
            //物體移動到當前位置
            ThrowItem.position = current_pos;

            //下一速度 = 當前速度 + 重力 * Time.Deltatime + 阻力 * Time.Deltatime
            next_vel = vel + Gravity * Time.deltaTime + resistance * Time.deltaTime;
            //當前速度 = 下一速度
            vel = next_vel;
        }
    }
    public void EndThrow() //物件刪除
    {
        itemEffect_pos = ThrowItem.position;
        Destroy(ThrowItem.gameObject);
        ThrowItem = null;
        isThrowing = false;
    }

    public void ItemExplode(string Explode)  //物件效果（動作事件）
    {
        Object o = Resources.Load(Explode);
        ItemEffect_obj = Instantiate((GameObject)o);

        if (ItemEffect_obj.name == "Obj_Ice(Clone)") 
        {
            Vector3 vec = itemEffect_pos;
            vec.y = 0;
            ItemEffect_obj.transform.position = vec;
        }
        else 
        {
            ItemEffect_obj.transform.position = itemEffect_pos;
        }
    }

    void DestroyItem(float t,string destroyEffect) 
    {
        if (timer > t)
        {
            Destroy(ItemEffect_obj);
            Object o = Resources.Load(destroyEffect);
            GameObject go = Instantiate((GameObject)o);
            go.transform.position = itemEffect_pos;
        }
    }

    /*
    
    >>> 應該讓拔剌撞到terrain就不能動 <<<

    1.動作串聯問題：狀態機
    2.是否做個全遊戲通用計時器
    3.補血
    4.UI

    時停()
    {
    動畫播放
    IK關掉
    道具進入CD
    UI操作
    怪物反應
    }

    特效&判定：
    1.取得物件最後的位置播放 >> 綁在物件上
    2.固定在人物前方某處啟動 >> 動畫事件


     */

}
