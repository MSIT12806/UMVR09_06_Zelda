using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene2CubeDrawLine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 point1 = transform.position + transform.forward * 11f + transform.right * -15f;
        Vector3 point2 = point1 + transform.right * 30f;
        Vector3 point3 = point2 + transform.forward * -22f;
        Vector3 point4 = point3 + transform.right * -30f;

        Debug.DrawLine(point1, point2, Color.red);
        Debug.DrawLine(point2, point3, Color.blue);
        Debug.DrawLine(point3, point4, Color.green);
        Debug.DrawLine(point4, point1, Color.green);

        TeleportSpace.Point1 = point1;
        TeleportSpace.Point2 = point2;
        TeleportSpace.Point3 = point3;
        TeleportSpace.Point4 = point4;

        //Debug.DrawLine(transform.position, transform.position + transform.forward * 10f, Color.green);
        //Debug.DrawLine(transform.position, transform.position + transform.forward * -10f, Color.green);
        //Debug.DrawLine(transform.position, transform.position + transform.right * 10f, Color.green);
        //Debug.DrawLine(transform.position, transform.position + transform.right * -10f, Color.green);
    }
}
public static class TeleportSpace
{
    public static Vector3 Point1;
    public static Vector3 Point2;
    public static Vector3 Point3;
    public static Vector3 Point4;
    //space.posision > point4
    //space.posision < point2
}