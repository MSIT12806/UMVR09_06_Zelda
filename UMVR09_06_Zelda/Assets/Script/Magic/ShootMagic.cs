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
    private void OnEnable()
    {
        force = (ObjectManager.MainCharacter.position - transform.position).normalized;
    }
    public LayerMask terrainMask;
    // Update is called once per frame
    void Update()
    {
        var offset = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            offset = 2f;
        }
        force += (ObjectManager.MainCharacter.position + (ObjectManager.MainCharacter.forward - (ObjectManager.MainCharacter.position - transform.position).normalized) * offset - transform.position).WithY(-1.5f).normalized * 5;
        transform.Translate(force.normalized / 5f);
        existSeconds -= Time.deltaTime;
        bool land = Physics.Raycast(transform.position, -Vector3.up, out var hit, 0.3f, terrainMask);
        if (existSeconds <= 0 || Vector3.Distance(transform.position, ObjectManager.MainCharacter.position.AddY(1)) < 0.4 || land)
        {
            var fx = Instantiate(ObjectManager.DragonFireBallExplosionFx);
            fx.transform.position = transform.position;
            fx.SetActive(true);
            gameObject.SetActive(false);
            NpcCommon.AttackDetection("Dragon", transform.position, transform.forward, 360f, 3f, false, new DamageData(50, Vector3.zero, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
        }
    }
}
