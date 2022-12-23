using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test1 : MonoBehaviour
{
    public Animator a;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var aa = a.GetAnimatorTransitionInfo(0);
        if (aa.IsName("1"))
        {
            Debug.Log("1");
        }
        if (aa.IsName("1 -> 1 1"))
        {
            Debug.Log("2");
        }
        if (aa.IsName("1 1"))
        {
            Debug.Log("3");
        }
    }
}
