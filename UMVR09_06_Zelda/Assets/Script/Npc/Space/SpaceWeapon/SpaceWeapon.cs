using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpaceWeapon : MonoBehaviour
{
    public ShootMagic[] smallBalls;
    public ShootMagic bigBall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SmallBallAttack(Vector3 position)
    {
        var smallBall = GetSmallBall();
        smallBall.existSeconds = 1.5f;
        smallBall.transform.position = position;
        smallBall.force = (ObjectManager2.MainCharacter.position - this.transform.position).WithY();
        smallBall.gameObject.SetActive(true);
    }
    public void BigBallAttack(Vector3 position)
    {
        bigBall.existSeconds = 3f;
        bigBall.transform.position = position;
        bigBall.force = (ObjectManager2.MainCharacter.position - this.transform.position).WithY();
        bigBall.gameObject.SetActive(true);
    }

    private ShootMagic GetSmallBall()
    {
        return smallBalls.First(i => i.gameObject.activeSelf == false);
    }
}
