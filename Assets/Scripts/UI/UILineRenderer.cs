using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasRenderer))]
public class UILineRenderer : Graphic
{
    public List<Vector2> points = new List<Vector2>();
    public float lineWidth = 3f;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (points.Count < 2) return;

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 p1 = points[i];
            Vector2 p2 = points[i + 1];

            Vector2 dir = (p2 - p1).normalized;
            Vector2 perp = new Vector2(-dir.y, dir.x) * (lineWidth * 0.5f);

            // Miter joint — sambungkan ujung segmen sebelumnya
            Vector2 perp1 = perp;
            Vector2 perp2 = perp;

            if (i > 0)
            {
                Vector2 prevDir = (p1 - points[i - 1]).normalized;
                Vector2 miter = (perp + new Vector2(-prevDir.y, prevDir.x) * (lineWidth * 0.5f)).normalized;
                float length = lineWidth * 0.5f / Mathf.Max(Vector2.Dot(miter, new Vector2(-dir.y, dir.x)), 0.1f);
                perp1 = miter * length;
            }

            if (i < points.Count - 2)
            {
                Vector2 nextDir = (points[i + 2] - p2).normalized;
                Vector2 miter = (perp + new Vector2(-nextDir.y, nextDir.x) * (lineWidth * 0.5f)).normalized;
                float length = lineWidth * 0.5f / Mathf.Max(Vector2.Dot(miter, new Vector2(-dir.y, dir.x)), 0.1f);
                perp2 = miter * length;
            }

            int idx = vh.currentVertCount;
            UIVertex v = UIVertex.simpleVert;
            v.color = color;

            v.position = p1 + perp1; vh.AddVert(v);
            v.position = p1 - perp1; vh.AddVert(v);
            v.position = p2 - perp2; vh.AddVert(v);
            v.position = p2 + perp2; vh.AddVert(v);

            vh.AddTriangle(idx, idx + 1, idx + 2);
            vh.AddTriangle(idx, idx + 2, idx + 3);
        }
    }
}