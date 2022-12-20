using Ron;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BigBallExplosion : MonoBehaviour
{
    ParticleSystem keeping;
    public ParticleSystem[] explosions;
    // Start is called before the first frame update

    void Start()
    {
    }

    private void OnEnable()
    {
        if (keeping == null) keeping = transform.FindAnyChild<ParticleSystem>("Flash_Main");
        keeping.Play();
    }
    private void OnDisable()
    {
        keeping.Stop();
        Explosion();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void Explosion()
    {
        //做一次攻擊判定
        //爆炸
        var exp = explosions.First(i => i.gameObject.activeSelf == false);
        exp.transform.position = transform.position;
        exp.gameObject.SetActive(true);
    }
}
