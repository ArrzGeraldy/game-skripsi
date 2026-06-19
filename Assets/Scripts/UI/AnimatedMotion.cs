using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum MotionType
{
    Parabola,
    Vertical,
    Horizontal
}
public class AnimatedMotion : MonoBehaviour
{
    public MotionType motionType;
    public RectTransform target;
    public TextMeshProUGUI labelVy = null;
    public TextMeshProUGUI labelVx = null;

    public float speed = 100f;
    public float height = 200f;
    public float gravity = 98f;
    public float angle = 0;
    public int direction = 1;
    public float delayReset = 1f;

    public float t;
    private Vector2 startPos;
    private UILineRenderer uiLine; 
    bool landed;
    private bool isWaiting = false;
    public float waitTimer = 0f;
    public bool isAnimated = true;


    void Awake()
    {
        startPos = target.anchoredPosition;
        GetComponent<UILineRenderer>().material = new Material(Shader.Find("UI/Default"));
        uiLine = GetComponent<UILineRenderer>();

     
    }

    void Update()
    {
        if(!isAnimated) return;

        switch (motionType)
        {
            case MotionType.Parabola:
                AnimateParabola();
                break;

            case MotionType.Vertical:
                AnimateVertical();
                break;

            case MotionType.Horizontal:
                AnimateHorizontal();
                break;
        }
    }

    void AnimateParabola()
    {
        t += Time.deltaTime;

        float vx = speed * Mathf.Cos(angle * Mathf.Deg2Rad);
        float vy = speed * Mathf.Sin(angle * Mathf.Deg2Rad);

        
        float displayV =(speed / 10f) * Mathf.Sin(angle * Mathf.Deg2Rad)
                            - ((gravity / 10f) * t);
        if (labelVx != null)
            labelVx.text = $"v<sub>x</sub> = {(speed / 10 * Mathf.Cos(angle * Mathf.Deg2Rad)):F1} m/s";    

        if (labelVy != null)
            labelVy.text =  $"v<sub>y</sub> = {displayV:F1} m/s";;

        // posisi
        float x = vx * t;
        float y = (vy * t) - (0.5f * gravity * t * t);

        //  grounded
        if (y < 0)
        {
            y = 0;

            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = 0;
            }

            waitTimer += Time.deltaTime;

            // move horizontal
            target.anchoredPosition = startPos + new Vector2(x, 0);

            if (waitTimer >= delayReset)
            {
                ResetAnimation();
            }

            return;
        }

        // normal parabola
        target.anchoredPosition = startPos + new Vector2(x, y);

        uiLine.points.Add(startPos + new Vector2(x, y));
        uiLine.SetVerticesDirty();
    }


    private float prevV = 0f;
    private bool isMovingUp = true;
    bool isPausedAtPeak = false;
    float peakPauseTimer = 0f;

    void AnimateVertical()
    {
        if (isPausedAtPeak)
        {
            peakPauseTimer += Time.deltaTime;
            if (peakPauseTimer >= 1f)
                isPausedAtPeak = false;
            return;
        }

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= delayReset)
                ResetAnimation();
            return;
        }

        t += Time.deltaTime;
        float y = (speed * t) - (0.5f * gravity * t * t);
        float v = speed - (gravity * t);

        float displayV = v / 6f;
        if (labelVy != null)
            labelVy.text = $"v = {displayV:F1} m/s";

        // ── Deteksi titik puncak: transisi dari positif ke nol/negatif ──
        if (prevV > 0 && v <= 0)
        {
            isPausedAtPeak = true;
            peakPauseTimer = 0f;
            prevV = 0f;
            Debug.Log("Titik puncak: " + target.anchoredPosition.y);
            return;
        }

        prevV = v; // simpan v frame ini

        // ── Grounded ──
        if (y < 0)
        {
            y = 0;
            target.anchoredPosition = startPos + new Vector2(0, y);
            isWaiting = true;
            waitTimer = 0f;
            return;
        }

        // ── Normal ──
        target.anchoredPosition = startPos + new Vector2(0, y);
        uiLine.points.Add(startPos + new Vector2(0, y));
        uiLine.SetVerticesDirty();
    }


    void AnimateHorizontal()
    {
        // Diam dulu 0.5 detik sebelum reset
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= 0.5f)
            {
                isWaiting = false;
                waitTimer = 0f;
                ResetAnimation();
            }
            return;
        }

        t += Time.deltaTime;
        float x = Mathf.Min(t * speed, height) * direction;

    

        target.anchoredPosition = startPos + new Vector2(x, 0);
        uiLine.points.Add(target.anchoredPosition);
        uiLine.SetVerticesDirty();

        // Sudah sampai ujung, mulai nunggu
        if (x >= height * direction)
        {
            isWaiting = true;
        }
    }

    public void ResetAnimation()
    {
        t = 0;
        isWaiting = false;
        target.anchoredPosition = startPos;
        uiLine.points.Clear();
        uiLine.SetVerticesDirty();
        isPausedAtPeak = false;
        peakPauseTimer = 0f;  
        prevV = speed; 
    }




}
