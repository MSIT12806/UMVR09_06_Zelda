using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetApple : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }
    int floatFrameCount = 150;
    // Update is called once per frame
    void Update()
    {
        var distance = Vector3.Distance(ObjectManager.MainCharacter.position, transform.position);
        if (distance <= 2)
        {
            Debug.Log("¸ÉÄ«ªG");
            PicoManager.AppleCount++;
            Destroy(gameObject);
        }
    }
}
