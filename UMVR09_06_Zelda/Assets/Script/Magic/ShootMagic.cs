using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShootMagic : MonoBehaviour
{
    [HideInInspector] public Vector3 force;
    [HideInInspector] public float existSeconds;
    public float speedDecrease = 5f;
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        var currentScene = SceneManager.GetActiveScene();
        var currentSceneName = currentScene.name;
        if (currentSceneName == "NightScene")
        {
            isNightScene = true;
        }
        else
        {
            isNightScene = false;
        }

        if (isNightScene)
        {
            target = ObjectManager.MainCharacter;
        }
        else
        {
            target = ObjectManager2.MainCharacter;
        }
    }
    private void OnEnable()
    {
        force = (target.position - transform.position).normalized;
    }
    public LayerMask terrainMask;
    private bool isNightScene;

    // Update is called once per frame
    void Update()
    {
        var offset = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            offset = 2f;
        }
        force += (target.position + (target.forward - (target.position - transform.position).normalized) * offset - transform.position).WithY(-1.5f).normalized * 5;
        transform.Translate(force.normalized / speedDecrease);
        existSeconds -= Time.deltaTime;
        bool land = Physics.Raycast(transform.position, -Vector3.up, out var hit, 0.3f, terrainMask);
        if (existSeconds <= 0 || Vector3.Distance(transform.position, target.position.AddY(1)) < 0.4 || land)
        {
            //NpcCommon.AttackDetection("Dragon", transform.position, transform.forward, 360f, 3f, false, new DamageData(50, Vector3.zero, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
            if (ObjectManager.DragonFireBallExplosionFx == null) return;
            var fx = Instantiate(ObjectManager.DragonFireBallExplosionFx);
            fx.transform.position = transform.position;
            fx.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
