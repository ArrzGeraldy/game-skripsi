using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForceRb : MonoBehaviour
{
    public float t = 1;

    void Start()
    {
        Time.timeScale = t;
    }
   
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = Time.timeScale > 0 ? 0 : t;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ScreenCapture.CaptureScreenshot("screenshot.png");
            Debug.Log("Screenshot diambil");
        }
    }


}