using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiUnitTest : MonoBehaviour
{
    // Start is called before the first frame update

    public float Value;
    public readonly float MaxValue = 1000;
    public Image WeakFull;
    public Image WeakCrack;
    void Start()
    {
        Value = MaxValue;
        WeakFull.fillAmount = 1;
        WeakCrack.fillAmount = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            Value--;
        }

        RefreshWeak();
    }

    private void RefreshWeak()
    {
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
    }
}
