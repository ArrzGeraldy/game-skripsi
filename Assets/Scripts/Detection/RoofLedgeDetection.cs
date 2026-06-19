using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoofLedgeDetection : MonoBehaviour
{
    public int rayAmount = 10;
    public float rayLength = 0.5f;
    public float rayOffset = 0.15f;
    public float rayHeight = 1.7f;

    public RaycastHit rayLedgeHit;  
    public RaycastHit rayDownHit;
    public bool isDetected;
    PlayerClimb playerClimb;

    public float multi;


    void Start()
    {
        playerClimb = GetComponent<PlayerClimb>();
    }

    public void Detect()
    {

        for(int i = 0; i < rayAmount; i++)
        {
            Vector3 rayPos = transform.position + Vector3.up * 0.5f + transform.forward * rayOffset * i;

            Debug.DrawRay(rayPos, Vector3.down * rayLength ,Color.black);

            if(Physics.Raycast(rayPos, Vector3.down, out rayDownHit, rayLength, playerClimb.ledgeLayer))
            {
                Debug.DrawRay(rayDownHit.point + transform.forward * multi, -transform.forward, Color.red);
                
                if(Physics.Raycast(rayDownHit.point + transform.forward * multi, -transform.forward, out rayLedgeHit, 1, playerClimb.ledgeLayer))
                {
                    isDetected = true;
                    if(Input.GetKeyDown(KeyCode.C))
                    {
                        transform.rotation = Quaternion.LookRotation(-rayLedgeHit.normal);
                        StartCoroutine(DropToHang());
                        
                    }
                }
                else
                {
                    isDetected = false;
                }
                break;

            }
        }
    }

    IEnumerator DropToHang()
    {
        playerClimb.anim.CrossFade("Drop To Freehang", 0.2f);
        isDetected = false;
        playerClimb.isClimbing = true;
        playerClimb.PlayerState2 = PlayerState2.Climb;

        yield return null;
    }


    void OnDrawGizmos()
    {
        if(rayLedgeHit.point != null)
        {
            Gizmos.color = Color.black;

            Gizmos.DrawSphere(rayLedgeHit.point, 0.05f);
        }
    }

}
