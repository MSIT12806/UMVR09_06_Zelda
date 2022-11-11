using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKWeapon : MonoBehaviour
{
    public Transform TargetWeapon = null;
    public float weaponRotateAngle = 45.0f;

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
        Vector3 weaponUp = (RightPalmObj.position- LeftPalmObj.position).normalized;
        TargetWeapon.up = weaponUp;

        //讓武器沿自身的y軸旋轉
        //TargetWeapon.rotation= Quaternion.AngleAxis(weaponRotateAngle, weaponUp);
        //問題：武器Y軸不會對齊weaponUp

        //武器的旋轉 = 當前的旋轉值+  [以weaponUp為軸心的y軸旋轉<<???]
        //Vector3 weaponRotateNow = TargetWeapon.transform.rotation.eulerAngles;

        //武器的位置 = 右手掌+武器y軸*距離
        TargetWeapon.position = RightPalmObj.position + (weaponUp* RightHandHeight);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(TargetWeapon.localPosition, TargetWeapon.up*20);
    }
}
