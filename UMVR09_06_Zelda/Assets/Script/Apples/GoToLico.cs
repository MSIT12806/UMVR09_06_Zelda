using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToLico : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 vel;


    void Start()
    {
        vel = (Vector3Extension.GetRandomDirection().AddY(1)) * 0.5f;
    }
    int floatFrameCount = 150;
    // Update is called once per frame
    void Update()
    {
        FreeFall();
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



    float terrainHeight = float.MinValue;
    public bool grounded;
    [SerializeField] LayerMask terrainLayer;
    void FreeFall()
    {
        terrainHeight = TerrainY();
        if (!grounded)
        {
            grounded = !EasyFalling.Fall(transform, ref vel, EndingYValue: terrainHeight);
        }
    }

    float TerrainY()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + (Vector3.up * 1f), Vector3.down, out hitInfo, 5f, terrainLayer))
        {
            return hitInfo.point.y;
        }

        return float.MinValue;
    }
}
