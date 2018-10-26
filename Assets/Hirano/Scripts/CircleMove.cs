using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMove : MonoBehaviour
{
    private float speed;
    private float radius;
    private float yPosition;
    float x;
    float y;
    float z;

    // Use this for initialization
    void Start()
    {
        speed = 1.5f;
        radius = 2.0f;
        yPosition = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        x = radius * Mathf.Sin(Time.time * speed);
        y = yPosition;
        z = radius * Mathf.Cos(Time.time * speed);

        transform.position = new Vector3(x, y, z);
    }
}
