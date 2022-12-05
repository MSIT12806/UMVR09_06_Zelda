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
        if (existSeconds <= 0 || Vector3.Distance(transform.position, ObjectManager.MainCharacter.position) < 1)
        {
            gameObject.SetActive(false);
            NpcCommon.AttackDetection("Pico", transform.position, transform.forward, 360f, 5f, false, new DamageData(100, Vector3.zero, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
        }
    }
}
