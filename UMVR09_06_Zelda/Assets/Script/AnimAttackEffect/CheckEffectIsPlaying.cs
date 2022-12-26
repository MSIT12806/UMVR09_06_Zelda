using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEffectIsPlaying : MonoBehaviour
{
    public SpaceManager space;
    ParticleSystem effect;
    // Start is called before the first frame update
    void Start()
    {
        effect = this.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (effect.isPlaying)
        {
            space.EffectPlaying.Add(this.GetComponent<ParticleSystem>());

            //Debug.Log(3333333333);
        }
        if (effect.isStopped)
        {
            space.EffectPlaying.Remove(effect);
        }
    }
}
