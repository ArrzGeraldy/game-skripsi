using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class KinematicsVisualizer : MonoBehaviour
{
    LineRenderer lr;

    [Header("Line")]
    [SerializeField] Transform bulletOut;
    [SerializeField] TextMeshPro label;
    [SerializeField] float length;

    [Header("Line Config")]
    public float startWidth = 0.05f;
    public float endWidth = 0.05f;

    public Vector3 offset = new Vector3(0,0, 0);

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Sprites/Default"));

        lr.startWidth = startWidth;
        lr.endWidth = endWidth;

        // pos
        Vector3 pos = bulletOut.position + offset;

        lr.SetPosition(0, pos);

        Vector3 endPos = pos + (bulletOut.forward * length);
        // point 2
        lr.SetPosition(1, endPos);



    }

    // Update is called once per frame
    void Update()
    {
        // pos
        Vector3 pos = bulletOut.position + offset;

        lr.SetPosition(0, pos);

        Vector3 endPos = pos + (bulletOut.forward * length);
        // point 2
        lr.SetPosition(1, endPos);
    }
}
