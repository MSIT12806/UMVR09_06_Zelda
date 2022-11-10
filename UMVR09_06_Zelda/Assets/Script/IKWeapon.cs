using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKWeapon : MonoBehaviour
{
    public Transform TargetWeapon = null;

    //�k��
    public Transform RightPalmObj = null;
    public float RightHandHeight = 0.0f;

    //����
    public Transform LeftPalmObj = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //�Z����y�b = �V�q(�k���m-�����m)
        Vector3 weaponUp = RightPalmObj.position- LeftPalmObj.position;
        //�k�⪺��m = �k��x+�Z��y�b*�Z��
        TargetWeapon.position = RightPalmObj.position + (weaponUp* RightHandHeight);
        TargetWeapon.up = weaponUp;
    }
}
