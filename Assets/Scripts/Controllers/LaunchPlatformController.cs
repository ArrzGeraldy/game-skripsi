using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPlatformController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool error = false;
    [SerializeField] private bool onCollision = false;

    public Canvas InputParabola;
    public Canvas DistanceHint;
    public float offsetZ = 0.3f;
    public Transform interact;
    [SerializeField] Vector3 sideViewEuler = new Vector3(15, -90, 0);
    


    void Start()
    {
        InputParabola.gameObject.SetActive(false);
        SetDistanceHint(false);

        if(!Player.Instance)
        {
            Debug.LogError("Player Instance NULL");
            error = true;
        }
    }

    void Update()
    {
        // if(onCollision && Input.GetKeyDown(KeyCode.Escape))
        // {
        //     Player.Instance.SwitchVCam(VCamType.VCAM_3PERSON);
        //     InputParabola.gameObject.SetActive(false);
        //     SetDistanceHint(false);
        // }
    }

    void OnTriggerEnter(Collider col)
    {
        if(error) return;
        if(col.CompareTag("Player") && !onCollision)
        {
            Player.Instance.cc.enabled = false;
            Player.Instance.transform.position = new Vector3(interact.position.x, Player.Instance.transform.position.y, interact.position.z);
            Player.Instance.cc.enabled = true;

            Player.Instance.SwitchVCam(VCamType.VCAM_SIDEVIEW);
            Player.Instance.SetSideViewRotation(sideViewEuler);
            InputParabola.gameObject.SetActive(true);
            SetDistanceHint(true);

            onCollision = true;

        }
    }

    void OnTriggerExit(Collider col)
    {
        if (error) return;

        if (col.CompareTag("Player") && onCollision)
        {
            Debug.Log("EXIT: " + col.tag);
            Debug.Log("Pos Exit: " + Player.Instance.transform.position);


            onCollision = false;

        }
    }

    void SetDistanceHint(bool val)
    {
        if(DistanceHint != null)
            DistanceHint?.gameObject.SetActive(val);
    }
}
