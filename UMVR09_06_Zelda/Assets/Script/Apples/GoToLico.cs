using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToLico : MonoBehaviour
{
    // Start is called before the first frame update



    void Start()
    {
        
    }
    int floatFrameCount = 150;
    // Update is called once per frame
    void Update()
    {
        if (floatFrameCount > 0)
        {
            floatFrameCount--;
            return;
        }
        else
        {
            var direction = (ObjectManager.MainCharacter.position - transform.position).normalized;
            transform.position += direction * 0.1f;

        }

    }
}
