using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJump : MonoBehaviour
{
    CharacterController cc;
    
    [Header("Jump Settings")]
    public float jumpHeight1 = 1.5f;
    public float jumpHeight2 = 2.2f;
    public float jumpHeight3 = 2.8f;
    public float height = 0;


    public float gravity = -9.81f;
    public float fallMultiplier = 1.5f; 

    float verticalVelo = 0;
    public Animator anim;
    public int jumpCount = 0;
    public float jumpBuffer = 0f;
    public float jumpBufferTime = 0.15f;

    Camera cam;
    void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        if (jumpBuffer > 0f)
            jumpBuffer -= Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            jumpBuffer = jumpBufferTime;
        }
        

        if (cc.isGrounded && jumpBuffer > 0f)
        {
            jumpBuffer = 0f;

            verticalVelo = -2f;
           
            jumpCount++;
            anim.SetInteger("jumpCount", jumpCount);
            anim.SetTrigger("jump");
            height = jumpCount switch
            {
                1 => jumpHeight1,
                2 => jumpHeight2,
                3 => jumpHeight3,
                _ => 1f
            };

            verticalVelo = Mathf.Sqrt(-2f * gravity * height);
            Debug.Log("jump - " + jumpCount + " | height: " + height + " | verticalVelo: " + verticalVelo);

            if(jumpCount >= 3)
                jumpCount = 0;

        }
        else
        {
            if(verticalVelo < 0f)
            {
                verticalVelo += gravity * 2f * Time.deltaTime;
            }
            else if(verticalVelo > 0f && !Input.GetKey(KeyCode.Space))
            {
                verticalVelo += gravity * 1.5f * Time.deltaTime;
            }
            else
            {
                verticalVelo += gravity * Time.deltaTime;
                
            }

        }


        cc.Move(new Vector3(0, verticalVelo * Time.deltaTime, 0));
        Move();

        anim.SetBool("grounded", cc.isGrounded);
        anim.SetFloat("speed", currSpd/5f);
    }

    float currSpd = 0f;
    float accleration = 8f;

    void Move()
    {
        // handle movement
        float z = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");
        // check direction
        Vector3 dir = cam.transform.right * x + cam.transform.forward * z;
        dir.y = 0;
        dir = dir.normalized;


        float targetSpeed = 0f;

        if(dir.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 3 * Time.deltaTime);

            targetSpeed = Input.GetKey(KeyCode.LeftShift) ? 3f : 5f ;
        }


        // calculate accleration
        float deltaSpd = targetSpeed - currSpd;
        float step = accleration * Time.deltaTime;
        deltaSpd = Mathf.Clamp(deltaSpd, -step, step);
        currSpd += deltaSpd;



        Vector3 move = dir * currSpd;
        move.y = 0;

        cc.Move(move * Time.deltaTime);
      
    }


}
