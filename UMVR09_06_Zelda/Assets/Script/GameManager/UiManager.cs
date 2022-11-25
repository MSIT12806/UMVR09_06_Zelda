using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ron;

public class UiManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform HpUi;
    float currentHp;
    float currentFever;
    void Start()
    {
        HpUi = transform.FindAnyChild<Transform>("MainCharacterHP");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetHpBar()
    {

    }
    void SetFeverBar()
    {

    }
}
