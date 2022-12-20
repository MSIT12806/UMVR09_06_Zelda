using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShootMagic : MonoBehaviour
{
    [HideInInspector] public Vector3 force;
    [HideInInspector] public float existSeconds;
    float exist;
    public float speedDecrease = 5f;
    public float withY = -1.5f;
    public float redius = 3f;
    Transform target;
    private void Awake()
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


    }
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnEnable()
    {
        initialize = false;
        exist = existSeconds;
    }
    public LayerMask terrainMask;
    private bool isNightScene;
    bool initialize;
    // Update is called once per frame
    void Update()
    {
        if (initialize==false)
        {
            if (target == null)
            {
                if (isNightScene)
                {
                    target = ObjectManager.MainCharacter;
                }
                else
                {
                    target = ObjectManager2.MainCharacter;
                }
            }
            force = (target.position - transform.position).normalized;
            initialize = true;
        }
        var offset = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            offset = 2f;
        }
        force += (target.position + (target.forward - (target.position - transform.position).normalized) * offset - transform.position).WithY(withY).normalized * 5;
        transform.Translate(force.normalized / speedDecrease);
        exist -= Time.deltaTime;
        bool land = Physics.Raycast(transform.position, -Vector3.up, out var hit, 0.3f, terrainMask);
        if (exist <= 0 || Vector3.Distance(transform.position, target.position.AddY(1)) < 0.4 || land)
        {
            NpcCommon.AttackDetection("Dragon", transform.position, transform.forward, 360f, redius, false, new DamageData(50, Vector3.zero, HitType.Heavy, DamageStateInfo.NormalAttack), "Player");
            try
            {
                var fx = Instantiate(ObjectManager.DragonFireBallExplosionFx);
                fx.transform.position = transform.position;
                fx.SetActive(true);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
            finally
            {
                End();
            }
        }
    }

    public void End()
    {
        gameObject.SetActive(false);
    }
}
