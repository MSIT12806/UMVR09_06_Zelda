using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObj : MonoBehaviour
{
    public Vector3 initVel;
    float terrainHeight = float.MinValue;
    bool grounded;
    [SerializeField] LayerMask terrainLayer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        terrainHeight = StandOnTerrain();
        if (!grounded)
            grounded = !EasyFalling.Fall(transform, ref initVel, EndingYValue: terrainHeight);
    }

    float StandOnTerrain()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position + (Vector3.up * 1f), Vector3.down, out hitInfo, 5f, terrainLayer))
        {
            return hitInfo.point.y;
        }

        return float.MinValue;
    }
}
