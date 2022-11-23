using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiNpc : ScriptableObject, IHp
{
    public Transform transform;
    List<GameObject> enemies = new List<GameObject>();//lookat, chase, attack, escape...
    AiState state;
    public float Hp { get; set; }

    public float Brave; //勇氣值，初始化時賦予，越高則攻擊性越強。
    public bool Dizzy; //暈眩值，受到攻擊時賦予，眩暈後會隨機告知持續幾秒。

    public AiNpc(Transform t)
    {
        state = new IdelState();
        transform = t;
    }
}
