using Ron;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LevelHandler : MonoBehaviour
{
    public int level;
    bool finished;
    bool done;

    public PlayableDirector timeline;
    ParticleSystem bean;
    ParticleSystem light;
    List<Transform> column = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        bean = transform.FindAnyChild<ParticleSystem>("StrongholdBeam");
        light = transform.FindAnyChild<ParticleSystem>("LightningCircle");
        column.Add(transform.FindAnyChild<Transform>("obelisk_main"));
        column.Add(transform.FindAnyChild<Transform>("obelisk_side"));
        column.Add(transform.FindAnyChild<Transform>("obelisk_plate"));
    }

    public void FinishThisLevel()
    {
        bean.gameObject.SetActive(false);
        light.gameObject.SetActive(false);
        foreach (var item in column)
        {
            var ren = item.GetComponent<Renderer>();
            foreach (var i in ren.materials)
            {
                    i.SetColor("_EmissionColor", new Color(0, 0, 0, 0));
            }
        }

        timeline.Play();
    }
}
