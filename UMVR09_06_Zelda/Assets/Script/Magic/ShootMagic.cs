using Ron;
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
    public LayerMask terrainMask;
    // Update is called once per frame
    void Update()
    {
        force += (ObjectManager.MainCharacter.position + ObjectManager.MainCharacter.forward * 2 - transform.position).WithY(-1f).normalized * 5;
        transform.Translate(force.normalized / 8f);
        existSeconds -= Time.deltaTime;
        bool land = Physics.Raycast(transform.position, -Vector3.up, out var hit, 0.3f, terrainMask);
        if (existSeconds <= 0 || Vector3.Distance(transform.position, ObjectManager.MainCharacter.position.AddY(1)) < 0.4 || land)
        {
            var fx = Instantiate(ObjectManager.DragonFireBallExplosionFx);
            fx.transform.position = transform.position;
            fx.SetActive(true);
            gameObject.SetActive(false);
            NpcCommon.AttackDetection("Pico", transform.position, transform.forward, 360f, 3f, false, new DamageData(100, Vector3.zero, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
        }
    }
}
