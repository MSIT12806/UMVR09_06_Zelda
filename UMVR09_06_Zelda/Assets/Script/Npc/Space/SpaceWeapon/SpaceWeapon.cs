using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpaceWeapon : MonoBehaviour
{
    public GameObject[] smallBalls;
    public GameObject bigBall;
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
        GameObject smallBall = GetSmallBall();
        smallBall.transform.position = position;
        smallBall.SetActive(true);
    }
    public void BigBallAttack(Vector3 position)
    {
        bigBall.transform.position = position;
        bigBall.SetActive(true);
    }

    private GameObject GetSmallBall()
    {
        return smallBalls.First(i => i.activeSelf == false);
    }
}
