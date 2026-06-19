using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashRibbon : MonoBehaviour
{
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    bool isMoving = false;

    Mesh mesh;
    public float baseRadius = 0.2f;

    float timer = 0f;
    float _duration = 3f;

    public Vector3 p0 = new Vector3(2, 0, 0);
    public Vector3 p1 = new Vector3(0.5f, 0.0f, 2);
    public Vector3 p2 = new Vector3(-0.5f, 0.0f, 2);
    public Vector3 p3 = new Vector3(-2, 0, 0);
    Material mat;


    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mat = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if(isMoving)
        {
            transform.position += transform.forward * 5f * Time.deltaTime;
            timer += Time.deltaTime;

            if(timer >= _duration)
                Destroy(gameObject);
        }
    }

    public void DrawSlashCubic(float _type = 1)
    {
   
        // Vector3 p0 = new Vector3(1.5f, 0, 0);
        // Vector3 p1 = new Vector3(0.3f, 0.0f, 1.5f);
        // Vector3 p2 = new Vector3(-0.3f, 0.0f, 1.5f);
        // Vector3 p3 = new Vector3(-1.5f, 0, 0);
        if(_type == 2)
        {
            p0 *= 1.4f;
            p1 *= 1.4f;
            p2 *= 1.4f;
            p3 *= 1.4f;
            baseRadius *= 1.8f;
        }

        mat.SetFloat("_Type", _type-1);
        Debug.Log("_Type param: " + _type);
        Debug.Log("_Type: " + (_type-1));

        int maxSegments = 20;
        int crossSegments = 8;

        for(int i = 0; i <= maxSegments; i++)
        {
            float t = (float)i/maxSegments;
            float currentRadius = baseRadius * Mathf.Sin(t * Mathf.PI);

            Vector3 center = cubicBezier(p0, p1, p2, p3, t);

             // menaikan t membuat vector lebih maju dari center
            Vector3 next_center = cubicBezier(p0, p1, p2, p3, t+0.01f);


            // simpan data untuk overlap
            // segmentsList.Add((center, next_center, currentRadius));
        
            // cari forward
            Vector3 forward = (next_center - center).normalized;

            Vector3 upLook = Vector3.up;
            // Jika forward terlalu dekat dengan arah up, gunakan arah forward lain (misal forward dunia)
            if (Mathf.Abs(Vector3.Dot(forward, upLook)) > 0.9f) 
            {
                upLook = Vector3.forward;
            }
            Vector3 right = Vector3.Cross(upLook, forward).normalized;
            Vector3 up = Vector3.Cross(forward, right).normalized;

            for(int j = 0; j <= crossSegments; j++)
            {
                float t_circle = (float)j/crossSegments;
                
                float radian = t_circle * Mathf.PI * 2f;

                Vector3 offset = ((right * Mathf.Cos(radian) ) + 
                     (up * Mathf.Sin(radian) )) *  currentRadius;

                vertices.Add(center + offset);
                uvs.Add(new Vector2(t, t_circle));
            }

        }

       int vCountPerRing = crossSegments + 1;

        for (int i = 0; i < maxSegments; i++)
        {
            for (int j = 0; j < crossSegments; j++) 
            {
                int curr = i * vCountPerRing + j;
                int next = i * vCountPerRing + (j + 1);
                int curr_up = (i + 1) * vCountPerRing + j;
                int next_up = (i + 1) * vCountPerRing + (j + 1);

                triangles.Add(curr);
                triangles.Add(curr_up);
                triangles.Add(next);

                triangles.Add(next);
                triangles.Add(curr_up);
                triangles.Add(next_up);
            }
        }
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        
        mesh.RecalculateNormals(); // Penting biar terlihat 3D
        mesh.RecalculateBounds();
        isMoving = true;
    }

    private Vector3 cubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 q0 = Vector3.Lerp(p0, p1, t);
        Vector3 q1 = Vector3.Lerp(p1, p2, t);
        Vector3 q2 = Vector3.Lerp(p2, p3, t);

        Vector3 r0 = Vector3.Lerp(q0, q1, t);
        Vector3 r1 = Vector3.Lerp(q1, q2, t);
        
        Vector3 p = Vector3.Lerp(r0, r1, t);

        return p;
    }
}
