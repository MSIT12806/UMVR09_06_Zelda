using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoll : MonoBehaviour
{
    // Start is called before the first frame update
    //GameObject target;
    void Start()
    {
        //target = ObjectManager2.MainCharacter.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float dis = (this.transform.position - ObjectManager2.MainCharacter.transform.position).magnitude;
        if(dis <= 10 && dis > 1f)
        {
            ObjectManager2.MainCharacter.transform.position
                = Vector3.Lerp(ObjectManager2.MainCharacter.transform.position, this.transform.position, 0.03f);
        }
    }
}
