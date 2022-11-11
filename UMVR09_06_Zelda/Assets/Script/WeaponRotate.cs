using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRotate : MonoBehaviour
{
    public float WeaponRotateAngle = 90.0f;

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(0, WeaponRotateAngle, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
