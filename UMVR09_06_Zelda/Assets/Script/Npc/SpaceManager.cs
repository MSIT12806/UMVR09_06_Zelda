using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceManager : MonoBehaviour, NpcHelper
{
    Npc npc;
    public SmallBall[] smallBalls;
    public float Hp { get => npc.Hp; set => npc.Hp = value; }

    public bool CanBeKockedOut => throw new System.NotImplementedException();

    public bool Dizzy => throw new System.NotImplementedException();

    public float MaxHp => throw new System.NotImplementedException();

    public float WeakPoint => throw new System.NotImplementedException();

    public float MaxWeakPoint => throw new System.NotImplementedException();

    public float Radius => 0.6f;

    public float CollisionDisplacement => 0;

    public string Name => "ªüÄõ®R";

    // Start is called before the first frame update
    Animator animator;
    void Awake()
    {
        var currentScene = SceneManager.GetActiveScene();
        var currentSceneName = currentScene.name;
        if (currentSceneName == "NightScene")
        {
            ObjectManager.StateManagers.Add(this.gameObject.GetInstanceID(), this);
        }
        else
        {
            ObjectManager2.StateManagers.Add(this.gameObject.GetInstanceID(), this);
        }
        animator = transform.GetComponent<Animator>();
        npc = transform.GetComponent<Npc>();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void GetHurt(DamageData damageData)
    {
        Debug.Log("hit");
        //aiState = new UsaoHurtState(transform.GetComponent<Animator>(), transform, damageData);
    }

    public void Move()
    {
        throw new System.NotImplementedException();
    }

    public void Turn(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }

    public void Look(Transform target)
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }
}
