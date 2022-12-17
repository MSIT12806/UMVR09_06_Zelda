using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeToMoonScene : MonoBehaviour
{
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float dis = (target.position - transform.position).magnitude;

        if(dis <= 2)
        {
            Application.LoadLevel(2);
        }
    }
}
