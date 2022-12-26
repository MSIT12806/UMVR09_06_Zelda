using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttach : MonoBehaviour
{
    public Transform playerRightHandBone;
    public GameObject weapon;

    void Start()
    {

        Transform weaponTrans = Instantiate(Resources.Load<Transform>("Weapons/Sword10"));
        //weapon.transform.parent = playerRightHandBone;
        weaponTrans.parent = playerRightHandBone;
        weaponTrans.localPosition = new Vector3(-0.07f, 0.1f, 0.0f);
        weaponTrans.localRotation = Quaternion.Euler(168, 90, 0);
        weaponTrans.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
