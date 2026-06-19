using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPadRotationalController : MonoBehaviour
{

    public bool onCollision = false;
    void Update()
    {
        // if(onCollision && Input.GetKeyDown(KeyCode.Escape))
        // {
        //     Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);
        // }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Player") && !onCollision)
        {
            onCollision = true;
            Player.Instance.transform.position = new Vector3(transform.position.x,  Player.Instance.transform.position.y, transform.position.z ); 
            Player.Instance.SwitchVCam(VCamType.VCAM_FPS);
        }
    }

    void OnTriggerExit(Collider col)
    {

        if (col.CompareTag("Player"))
        {
            onCollision = false;
        }
    }

}
