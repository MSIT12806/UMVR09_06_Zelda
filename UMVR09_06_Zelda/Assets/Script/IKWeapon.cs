using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKWeapon : MonoBehaviour
{
    public Transform TargetWeapon = null;
    public float weaponRotateAngle = 45.0f;

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
        Vector3 weaponUp = (RightPalmObj.position- LeftPalmObj.position).normalized;
        TargetWeapon.up = weaponUp;

        //���Z���u�ۨ���y�b����
        //TargetWeapon.rotation= Quaternion.AngleAxis(weaponRotateAngle, weaponUp);
        //���D�G�Z��Y�b���|���weaponUp

        //�Z�������� = ��e�������+  [�HweaponUp���b�ߪ�y�b����<<???]
        //Vector3 weaponRotateNow = TargetWeapon.transform.rotation.eulerAngles;

        //�Z������m = �k��x+�Z��y�b*�Z��
        TargetWeapon.position = RightPalmObj.position + (weaponUp* RightHandHeight);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(TargetWeapon.localPosition, TargetWeapon.up*20);
    }
}
