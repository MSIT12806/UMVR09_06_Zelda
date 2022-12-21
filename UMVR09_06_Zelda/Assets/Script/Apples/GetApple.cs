using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetApple : MonoBehaviour
{
    private ParticleSystem heal;
    // Start is called before the first frame update
    void Start()
    {
        heal = ObjectManager.MainCharacter.FindAnyChild<ParticleSystem>("AppleLine");
    }
    // Update is called once per frame
    void Update()
    {
        var distance = Vector3.Distance(ObjectManager.MainCharacter.position, transform.position);
        if (distance <= 2)
        {
            PicoManager.AppleCount++;
            Destroy(gameObject);
            heal.Play();
        }
    }
}
