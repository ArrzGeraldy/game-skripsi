using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDisplay : MonoBehaviour
{
    public Vector3 p0;
    public Vector3 p1;
    public Vector3 offset;
    public float length;

    void Start()
    {
        p0 = transform.position;
        p0.y -= transform.localScale.y/2;
        p0.x += transform.localScale.x/2;
        p0 += offset;
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.startWidth = 0.08f;
        lr.endWidth = 0.08f;
        lr.SetPosition(0, p0);
        p1 = p0;
        p1.y += length;
        lr.SetPosition(1, p1);
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.cyan;
        lr.endColor = Color.cyan;

    }

    void Update()
    {
        
    }
}
