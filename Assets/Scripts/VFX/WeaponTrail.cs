using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TrailSegment
{
    public Vector3 tipPos;
    public Vector3 basePos;
    public float bornTime;
}

public class WeaponTrail : MonoBehaviour
{
    [Header("References")]
    public Transform tipWeapon;
    public Transform baseWeapon;

    [Header("Buffers")]
    private List<TrailSegment> segments = new List<TrailSegment>();
    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    private List<int> triangles = new List<int>();

    [Header("Settings")]
    public float threshold = 0.01f;
    public float maxSegment = 20f;
    public bool emitting = false;
    float _duration = 1f;
    float _type = 0;

    Mesh mesh;
    Material mat;

    public float distance;
    public int count;
    Vector3 lastPos;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        lastPos = tipWeapon.position;
        mat = GetComponent<MeshRenderer>().material;

        // exit from parent
        transform.parent = null;
        transform.position = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
    }

    void Update()
    {

        if (!emitting)
            return;

        
        Vector3 currentPos = tipWeapon.position;

        distance = (currentPos - lastPos).magnitude;
        count = segments.Count;
        if(distance >= threshold)
        {
            segments.Add(new TrailSegment
            {
                tipPos = tipWeapon.position,
                basePos = baseWeapon.position,
                bornTime = Time.time
            });

            lastPos = currentPos;
        }
        

        // update segments
        for(int i = segments.Count - 1; i >= 0; i--)
        {
            float life = _duration - (Time.time - segments[i].bornTime);
            if(life <= 0.0f)
                segments.RemoveAt(i);
        }


        RebuildMesh();

        if(segments.Count > maxSegment)
            segments.RemoveAt(0);
       
    }



    void RebuildMesh()
    {
        if(segments.Count < 2)
        {
            mesh.Clear();
            return;
        }

        vertices.Clear();
        triangles.Clear();
        uvs.Clear();

         // add vertices
        for(int i = 0; i < segments.Count; i++)
        {
            TrailSegment seg = segments[i];

            float t = (float)i/(segments.Count-1);
            // Debug.Log(t);

            vertices.Add(seg.tipPos);
            vertices.Add(seg.basePos);

            uvs.Add(new Vector2(t, 1));
            uvs.Add(new Vector2(t, 0));

            
            if(i > 0)
            {
                int idx = i * 2;

                triangles.Add(idx - 2);
                triangles.Add(idx - 1);
                triangles.Add(idx);

                triangles.Add(idx);
                triangles.Add(idx - 1);
                triangles.Add(idx + 1);
            }
        }

        mat.SetFloat("_Type", _type);
        // apply mesh
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);

    }

    public void Play(float duration, float type = 0)
    {
        _type = type;
        emitting = true;
        _duration = duration;
    }

    public void Stop()
    {
        emitting = false;
        segments.Clear();
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        mesh.Clear();

    }
}
