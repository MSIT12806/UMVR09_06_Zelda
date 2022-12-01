using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMagic : MonoBehaviour
{
    public Vector3 force;
    public float existSeconds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(force);
        existSeconds -= Time.deltaTime;
        if (existSeconds <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
