using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ParabolaVisual : MonoBehaviour
{
    private LineRenderer lrRange;
    private LineRenderer lrHeight;
    
    public Transform bulletOut;
    public Transform target;
    public Transform targetHeight;
    public Vector3 offset;

    // Variabel penampung koordinat fix
    private Vector3 p0;
    private Vector3 p1;
    private float length;
    private Vector3 p0Height;
    private Vector3 p1Height;
    private float height;

    public TextMeshPro labelrRange;
    public TextMeshPro labelHeight;


    void Awake()
    {
          GameObject heightObj = new GameObject("LineHeight");
        Debug.Log(heightObj);
        heightObj.transform.parent = transform;
        lrHeight = heightObj.AddComponent<LineRenderer>();
    }

    void Start()
    {
        lrRange = gameObject.AddComponent<LineRenderer>();
        lrRange.positionCount = 2;
        lrRange.material = new Material(Shader.Find("Sprites/Default"));

        lrRange.startWidth = 0.05f;
        lrRange.endWidth = 0.05f;

        p0 = target.position; 
        p0.z = bulletOut.position.z;
        p1 = target.position; 

        length = (p1 - p0).magnitude;
        Debug.Log("Jarak Fix di Start: " + length);

        
        DrawLine();
      
        // height
        lrHeight.positionCount = 2;
        lrHeight.material = new Material(Shader.Find("Sprites/Default"));

        lrHeight.startWidth = 0.05f;
        lrHeight.endWidth = 0.05f;

        p0Height = bulletOut.position;
        p1Height = targetHeight.position + new Vector3(0, targetHeight.localScale.y / 2f, 0);
        height = p1Height.y - p0Height.y;
        Debug.Log("height: " + (p1Height.y - p0Height.y));

    }

    void Update()
    {
        DrawLine();
    }

    void DrawLine()
    {
        p0 = target.position; // p0 sekarang menyimpan posisi awal meriam (fixed)
        p0.z = bulletOut.position.z;
        p1 = target.position;    // p1 menyimpan posisi target tanah (fixed)

        // Hitung jarak horizontal/total murni di awal antara p1 dan p0
        length = (p1 - p0).magnitude;
        // Titik awal garis (p0 + offset agar konsisten)
        Vector3 startPos = p0 + offset;
        lrRange.SetPosition(0, startPos);

        // Titik akhir garis (Menggunakan arah moncong meriam saat ini, tapi dikali panjang 'length' yang FIX)
        // bulletOut.forward tetap dipakai agar garisnya ikut berputar mendongak ke atas mengikuti meriam
        Vector3 endPos = p1;
        lrRange.SetPosition(1, endPos);

        labelrRange.text = $"R = {length:F2}m";

        p0Height = bulletOut.position;
        p1Height = targetHeight.position + new Vector3(0, targetHeight.localScale.y / 2f, 0);
        lrHeight.SetPosition(0, p0Height);
        height = p1Height.y - p0Height.y;

        lrHeight.SetPosition(1, p0Height + Vector3.up * height);
        labelHeight.text = $"H = {height:F2}m";



    }
}
