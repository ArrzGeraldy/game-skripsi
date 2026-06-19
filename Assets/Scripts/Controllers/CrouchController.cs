using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchController : MonoBehaviour
{
    public LayerMask layer;
    public LayerMask groundLayer;
    RaycastHit hit;
    RaycastHit hitDown;
    public Vector3 rayOffset = new Vector3(0, 1.6f, 0);
    public float rayLength = 1f;
    public GameObject hintInput;
    Player player;


    void Start()
    {
        hintInput.SetActive(false);
        player = Player.Instance;
    }

    void Update()
    {
        Vector3 rayPos = transform.position + rayOffset;
        Debug.DrawRay(rayPos, transform.forward * rayLength);

        bool isDetected = Physics.Raycast(rayPos, transform.forward, out hit,rayLength, layer);
        bool isNormalState = player.state == PlayerState.Normal;


        if(isDetected && isNormalState)
        {
            Debug.DrawRay(hit.point +  Vector3.up, Vector3.down * 5f, Color.red);
            if(Physics.Raycast(hit.point +  Vector3.up, Vector3.down, out hitDown, 5f, groundLayer))
            {

                if(Input.GetKeyDown(KeyCode.C))
                {
                    StartCoroutine(Sliding());
                }
            }
        }

        hintInput.SetActive(Physics.Raycast(rayPos, transform.forward, rayLength+3, layer) && isNormalState);


        // MatchTarget();
    }

    IEnumerator Sliding()
    {
        player.anim.applyRootMotion = true;
        player.SetHasControl(false);
        transform.rotation = Quaternion.LookRotation(-hit.normal);
        player.anim.CrossFade("Sliding", 0.2f);
        player.SwitchVCam(VCamType.VCAM_SLIDING);


        var animState = player.anim.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSeconds(animState.length);

        player.SwitchVCam(VCamType.VCAM_3PERSON);
        player.SetHasControl(true);
        yield return null;
        player.anim.applyRootMotion = false;
    }


}
