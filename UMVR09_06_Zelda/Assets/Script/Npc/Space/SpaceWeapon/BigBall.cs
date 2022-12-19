using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBall : MonoBehaviour
{
    // Start is called before the first frame update
    Transform target;
    const float initVelocity = 8f;
    void Start()
    {
        target = ObjectManager2.MainCharacter;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) target = ObjectManager2.MainCharacter;

        target.forward = (target.position - transform.position).normalized;
        transform.position += target.forward * initVelocity * Time.deltaTime;
    }


}
