using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float yaw;
    public float pitch;
    [Range(1.0f, 10.0f)] public float sensitivity = 5f;

    public float fixedYPos;
    public Transform cameraFocus;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
        // transform.SetParent(null);
        fixedYPos = transform.position.y;

        Debug.Log(cameraFocus);
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

    public float upThreshold = 2f;
    public float downThreshold = 1.39f;
    public float speed = 5f;
    void LateUpdate()
    {
        // Transform playerPos = Player.Instance.transform;
        // float target = playerPos.position.y ;
        // float yDiff = target - transform.position.y;
        // Debug.Log(yDiff);

        // if(yDiff > upThreshold)
        // {
        //     Debug.Log("up");
        //     fixedYPos = Mathf.Lerp(fixedYPos, playerPos.position.y + downThreshold, speed * Time.deltaTime);
        // }
        // else if(yDiff < -downThreshold)
        // {
        //     Debug.Log("down");
        //     fixedYPos = Mathf.Lerp(fixedYPos, playerPos.position.y + downThreshold, speed * Time.deltaTime);
            
        // }
        // Debug.Log($"playerPos: {playerPos.position.y} | {transform.position.y}");

        // transform.position = new Vector3(cameraFocus.position.x, fixedYPos, cameraFocus.position.z);

        if(Player.Instance.lockInput) return;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

}
