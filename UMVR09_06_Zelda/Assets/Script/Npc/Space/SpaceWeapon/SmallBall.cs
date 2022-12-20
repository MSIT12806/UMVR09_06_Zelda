using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBall : MonoBehaviour
{
    // Start is called before the first frame update
    float speedPerSecond = 8.4f;
    readonly float Radius = 1f;
    readonly float Angle = 200f;
    readonly float attackSeconds = 4f;
    float nowAttackSecond;
    public SpaceManager spaceManager;
    public bool nowAttack;
    public SpaceWeapon weapons;

    public Vector3 startPos;
    void Start()
    {
        spaceManager.smallBallsAroundBody.Add(this.gameObject);
        startPos = this.transform.localPosition;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (nowAttackSecond > 0)
        //{
        //    nowAttackSecond -= Time.deltaTime;
        //    transform.forward = attackDirection;
        //    AttackBehavior();
        //    return;
        //}
        if (nowAttack)
        {
            Debug.Log(123456);
            Attack();
        }
        AroundBehavior();
    }
    private void OnEnable()
    {
            nowAttack = false;
    }
    void Attack()
    {
        if(transform.localPosition.x>0 && transform.localPosition.z < 0.05)
        {
            weapons.SmallBallAttack(transform.position);
            this.transform.localPosition = startPos;
            transform.gameObject.SetActive(false);
        }
    }

    void AroundBehavior()
    {
        attackDirection = (ObjectManager2.MainCharacter.position.WithY() - transform.position.WithY()).normalized;

        var distance = Vector3.Distance(transform.position.WithY(), ObjectManager2.Elena.position.WithY());
        var directionFaceSpace = (ObjectManager2.Elena.position.WithY() - transform.position.WithY()).normalized;
        if (distance >= Radius + 0.1)
        {
            transform.forward = directionFaceSpace;
            transform.position += transform.forward * speedPerSecond * Time.deltaTime;
            return;
        }

        if (distance <= Radius - 0.1)
        {
            transform.forward = -directionFaceSpace;
            transform.position += transform.forward * speedPerSecond * Time.deltaTime;
            return;
        }
        
        transform.RotateAround(ObjectManager2.Elena.position, ObjectManager2.Elena.up, Time.deltaTime * -Angle);

    }
    Vector3 attackDirection;
    void AttackBehavior()
    {

        transform.forward += ((ObjectManager2.MainCharacter.position + ObjectManager2.MainCharacter.forward) - transform.position).normalized;//
        transform.position += transform.forward * speedPerSecond * Time.deltaTime;
        //
        //1.以Space為圓心
        //2.以lico 為半徑
        //3.進行旋轉
        //預計攻擊4秒
    }

    //public void Attack()
    //{
    //    nowAttackSecond = attackSeconds;
    //}
}
