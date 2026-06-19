using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DistanceParabola : MonoBehaviour
{
    [Header("Refrences")]
    public Transform p0;
    public Transform p1;

    [Header("Label")]
    public TMP_Text labelDistance;
    public Vector3 labelOffset = new Vector3(0, -0.35f, 4);

    public string distanceStr = "10";
    


    void Start()
    {
        // Tambahkan LineRenderer component
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.startWidth = 0.08f;
        lr.endWidth = 0.08f;
        lr.SetPosition(0, p0.position + new Vector3(0, -0.35f, 0));
        lr.SetPosition(1, p1.position + new Vector3(0, -0.35f, 0));
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.red;
        lr.endColor = Color.red;
        float distance = Vector3.Distance(
            new Vector3(p0.position.x, 0f, p0.position.z),
            new Vector3(p1.position.x, 0f, p1.position.z)
        );


        RectTransform rt = labelDistance.GetComponent<RectTransform>();
        rt.position = ((p0.position + new Vector3(0, -0.35f, 0))  + (p1.position  + labelOffset)) / 2 + Vector3.down * 0.5f;
    

        rt.rotation = Quaternion.Euler(0f, -90f, 0f);

    }


    void Update()
    {
        float distance = Vector3.Distance(
            new Vector3(p0.position.x, 0f, p0.position.z),
            new Vector3(p1.position.x, 0f, p1.position.z)
        );

        // distance from bullet out to target
        // labelDistance.text = $"{distance:F1}m";

        // fix distance 10meter
        labelDistance.text = $"{distanceStr}m";

    }


}
