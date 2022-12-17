using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBall2 : MonoBehaviour
{
    public GameObject target;
    GameObject elena;
    // Start is called before the first frame update
    void Start()
    {
        elena = ObjectManager2.Elena.gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        float dis = (elena.transform.position - transform.position).magnitude;
        float move1 = 0.5f;
        float move2 = Mathf.Sqrt(move1 * move1 + dis * dis) - dis;
        transform.Translate(move1, 0, move2);
        transform.LookAt(elena.transform);
        Debug.Log(dis);
    }

    public void Attack(Vector3 target)
    {

    }
}
