using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraDumping : MonoBehaviour
{
     public CinemachineVirtualCamera vCam;
    private Cinemachine3rdPersonFollow thirdPerson;

    [Header("Y Threshold")]
    public float yUpThreshold = 1.5f;    // batas atas sebelum kamera follow
    public float yDownThreshold = 0.5f;  // batas bawah (lebih kecil = lebih cepat follow saat jatuh)
    public float dampingUp = 3f;         // lambat follow saat naik
    public float dampingDown = 8f;       // cepat follow saat jatuh

    private float baseY;
    private float currentOffsetY;

    void Start()
    {
        thirdPerson = vCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        baseY = transform.position.y;
        currentOffsetY = thirdPerson.ShoulderOffset.y;
    }

    void Update()
    {
        float yDiff = transform.position.y - baseY;
        float targetOffset = thirdPerson.ShoulderOffset.y;

        if (yDiff > yUpThreshold)
        {
            // Karakter naik melewati batas → follow lambat
            targetOffset = currentOffsetY + (yDiff - yUpThreshold);
            currentOffsetY = Mathf.Lerp(currentOffsetY, targetOffset, dampingUp * Time.deltaTime);
        }
        else if (yDiff < -yDownThreshold)
        {
            // Karakter jatuh melewati batas → follow cepat
            targetOffset = currentOffsetY + (yDiff + yDownThreshold);
            currentOffsetY = Mathf.Lerp(currentOffsetY, targetOffset, dampingDown * Time.deltaTime);
        }
        else
        {
            // Dalam threshold → kamera diam di Y
            currentOffsetY = Mathf.Lerp(currentOffsetY, 0f, dampingDown * Time.deltaTime);
        }

        var offset = thirdPerson.ShoulderOffset;
        offset.y = currentOffsetY;
        thirdPerson.ShoulderOffset = offset;
    }
}
