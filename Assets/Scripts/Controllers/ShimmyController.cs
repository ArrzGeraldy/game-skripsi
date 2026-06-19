using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShimmyController : MonoBehaviour
{
    PlayerClimb playerClimb;

    public float sphereRadius;
    public float sphereGap;

    public float rayHeight = 1.6f;
    public float rayLength = 1.0f;

    public bool canMoveRight;
    public bool canMoveLeft;

    public RaycastHit rayHit;
    Animator anim;


    void Awake()
    {
        anim = GetComponent<Animator>();
        playerClimb = GetComponent<PlayerClimb>();
    }


    void Update()
    {
        if(playerClimb.isClimbing)
        {
            Debug.DrawRay(transform.position + Vector3.up * rayHeight, transform.forward * rayLength, Color.magenta);
            Physics.Raycast(transform.position + Vector3.up * rayHeight, transform.forward, out rayHit, rayLength, playerClimb.ledgeLayer);
            CheckSphere();
        }
    }

    public bool leftBtn;
    public bool rightBtn;

    public float ledgeMoveSpeed = 0.5f;
    public float horizontalVal;

    void CheckSphere()
    {
        if(rayHit.point != null)
        {
            if(Physics.CheckSphere(rayHit.point + transform.right * sphereGap, sphereRadius, playerClimb.ledgeLayer))
            {
                rightBtn = Input.GetKey(KeyCode.D);
                canMoveRight = true;
            }
            else
            {
                rightBtn = false;
                canMoveRight = false;
            }
            
            if(Physics.CheckSphere(rayHit.point - transform.right * sphereGap, sphereRadius, playerClimb.ledgeLayer))
            {
                leftBtn = Input.GetKey(KeyCode.A);
                canMoveLeft = true;
            }
            else
            {
                leftBtn = false;
                canMoveLeft = false;
            }


            if(leftBtn)
                horizontalVal = -1;
            else if(rightBtn)
                horizontalVal = 1;
            else
                horizontalVal = 0;


            anim.SetFloat("speedLedge", horizontalVal, 0.05f, Time.deltaTime);
            
            transform.position += transform.right * horizontalVal * ledgeMoveSpeed *Time.deltaTime;
        }
    }


    void OnDrawGizmos()
    {
        if(rayHit.point != null)
        {
            Gizmos.color = Color.yellow;

            Gizmos.DrawSphere(rayHit.point + transform.right * sphereGap, sphereRadius);
            Gizmos.DrawSphere(rayHit.point - transform.right * sphereGap, sphereRadius);
        }
    }
}
