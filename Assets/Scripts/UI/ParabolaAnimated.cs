using UnityEngine;

public class ParabolaAnimated : MonoBehaviour
{
    public RectTransform ball;

    public float speed = 200f;
    public float height = 300f;
    public float gravity = 200f;
    public int dotCount = 30;

    private float t;
    private Vector2 startPos;
    private UILineRenderer uiLine; // TAMBAH
    bool landed;
    float landedTimer;
    public float groundSlideTime = 1f;

    void Start()
    {
        startPos = ball.anchoredPosition;
        GetComponent<UILineRenderer>().material = new Material(Shader.Find("UI/Default"));
        uiLine = GetComponent<UILineRenderer>();

     
    }

    void Update()
    {
        AnimateParabola();
    }

    void AnimateParabola()
    {
        t += Time.deltaTime;

        float x = speed * t;
        float y = (height * t) - (gravity * t * t);

        // kalau sudah jatuh
        if (y < 0)
        {
            y = 0;

            if (!landed)
            {
                landed = true;
                landedTimer = 0;
            }

            landedTimer += Time.deltaTime;

            // geser horizontal terus
            ball.anchoredPosition = startPos + new Vector2(x, y);

            if (landedTimer >= groundSlideTime)
            {
                ResetAnimation();
            }

            return;
        }

        // normal parabola
        ball.anchoredPosition = startPos + new Vector2(x, y);

        uiLine.points.Add(startPos + new Vector2(x, y));
        uiLine.SetVerticesDirty();
    }


    void ResetAnimation()
    {
        t = 0;
        landed = false;
        ball.anchoredPosition = startPos;
        uiLine.points.Clear();
        uiLine.SetVerticesDirty();
    }


    void DrawLine()
    {
        uiLine.points.Clear();
        for (int i = 0; i < dotCount; i++)
        {
            float ti = i * 0.05f;;
            float x = speed * ti;
            float y = (height * ti) - (gravity * ti * ti);
            if (y < 0) break;
            uiLine.points.Add(startPos + new Vector2(x, y));
        }
        uiLine.SetVerticesDirty();
    }

    void DrawLineRes()
    {
        uiLine.points.Clear();

        float totalTime = (height / gravity) * 2f;
        int resolution = 100; // pisahkan dari dotCount, khusus untuk line

        for (int i = 0; i <= resolution; i++)
        {
            float ti = (totalTime / resolution) * i;
            float x = speed * ti;
            float y = (height * ti) - (gravity * ti * ti);
            uiLine.points.Add(startPos + new Vector2(x, y));
        }
        uiLine.SetVerticesDirty();
    }

    void DrawParabolaLine()
    {

        float totalDuration = height / gravity;
        float timeStep = totalDuration / (dotCount - 1);

        for (int i = 0; i < dotCount; i++)
        {
            float simulatedTime = i * timeStep;

            float x = speed * simulatedTime;
            float y = (height * simulatedTime) - (gravity * simulatedTime * simulatedTime);
            uiLine.points.Add(startPos + new Vector2(x, y));
        }
        uiLine.SetVerticesDirty();
    }
}