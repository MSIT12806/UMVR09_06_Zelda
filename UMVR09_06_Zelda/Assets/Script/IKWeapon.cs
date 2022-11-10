using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKWeapon : MonoBehaviour
{
    public Transform TargetWeapon = null;

    //右手
    public Transform RightPalmObj = null;
    public float RightHandHeight = 0.0f;

    //左手
    public Transform LeftPalmObj = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //武器的y軸 = 向量(右手位置-左手位置)
        Vector3 weaponUp = RightPalmObj.position- LeftPalmObj.position;
        //右手的位置 = 右手掌+武器y軸*距離
        TargetWeapon.position = RightPalmObj.position + (weaponUp* RightHandHeight);
        TargetWeapon.up = weaponUp;
    }
}
