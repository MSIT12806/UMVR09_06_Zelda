using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter : ScriptableObject, IHp, IGas
{
    public float Hp { get; set; }
    public float Gas { get; set; }

    public void Launch()
    {
        throw new System.NotImplementedException();
    }
}
