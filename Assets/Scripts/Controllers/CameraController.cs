using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float yaw;
    public float pitch;
    [Range(1.0f, 10.0f)] public float sensitivity = 5f;

    public float fixedYPos;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
    }

    void Update()
    {
        if(Player.Instance.lockInput)
        {
            yaw = 0f;
            pitch = 15f;
            return;
        }
        ;
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        yaw += mx * sensitivity;
        pitch -= my * sensitivity;
        pitch = Mathf.Clamp(pitch, -30f, 60f);
    }


    void LateUpdate()
    {
        if(Player.Instance.lockInput) return;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

}
