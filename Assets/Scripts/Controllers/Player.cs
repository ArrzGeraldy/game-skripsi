using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum VCamType
{
    VCAM_3PERSON,
    VCAM_SIDEVIEW,
    VCAM_FPS,
    VCAM_SLIDING,

}

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    [Header("Refrences")]
    public static Player Instance;
    public CharacterController cc;
    public Animator anim;
    public AudioSource audioSource;
    public CinemachineVirtualCamera VCam_3Person;
    public CinemachineVirtualCamera VCam_SideView;
    public CinemachineVirtualCamera VCam_FPS;
    public CinemachineVirtualCamera VCam_Sliding;
    Camera cam;
    public GameObject canvasMenuPlayer;

    
    [Header("Attributes")]
    public float maxHp = 100f;
    public float currentHp;

    [Header("Movement")]
    public float normalSpd = 3f;
    public float maxSpd = 6f;
    public float currSpd;
    public float smoothRot = 3f;
    public Vector3 velocity = Vector3.zero;
    public Vector3 externalForce = Vector3.zero;
    public float drag = 5f;
    Vector3 dir;
    public float accleration = 10f;

    [Header("Jumping")]
    public float jumpHeight1 = 1.5f;
    public float jumpHeight2 = 1.9f;
    public float jumpHeight3 = 2.5f;
    public bool isJumping = false;
    public float jumpTimer = 0f;
    public float jumpCooldown = 0.5f;
    public float fallJumpMultiplier = 1.5f;
    public int jumpCount = 0;
    public float jumpBuffer = 0f;
    public float jumpBufferTime = 0.15f;

    [Header("Control")]
    public bool lockMovement = false;
    public bool lockInput = false;
    [SerializeField] private bool hasControl = true;

    Quaternion targetRot;

    [Header("Player Grounded")]
    public bool Grounded = false;
    public float GroundedRadius = 0.28f;
    public float GroundedOffset = -0.14f;
    public LayerMask GroundLayers;
    // none
    [SerializeField]private float _verticalVelocity = 0f;
    private float Gravity = -9.81f;

    public bool canJump = false;
    private int _activeUICount = 0;

    public PlayerState state = PlayerState.Normal;



    void Awake()
    {
        cam = Camera.main;
        Instance = this;
        cc = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        anim.applyRootMotion = false;

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            lockInput = !lockInput;
            
            Time.timeScale = Time.timeScale >= 1f ? 0.3f : 1f;
        }


        // if(Input.GetKeyDown(KeyCode.Escape))
        // {
        //     // anim.SetFloat("speed", 0);
        //     lockInput = true;
        //     RegisterUI();
        //     Time.timeScale = 0f;
        //     canvasMenuPlayer.SetActive(true);
        // //     Application.Quit();
        // // #if UNITY_EDITOR
        // //     UnityEditor.EditorApplication.isPlaying = false;
        // // #endif
        // }
   

        GroundCheck();
        if(!hasControl) return;
        
        if(lockInput) return;
        JumpAndGravity();

        Move();


        anim.SetFloat("speed", currSpd/maxSpd);
        anim.SetBool("grounded", Grounded);
    }


    void GroundCheck()
    {
        Vector3 spherePos = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePos, GroundedRadius, GroundLayers);

    }

    void JumpAndGravity()
    {
      
        anim.SetBool("isJumping", isJumping);
        if(jumpTimer > 0f)
            jumpTimer -= Time.deltaTime;

        if(Grounded && jumpTimer <= 0f)
        {
            state = PlayerState.Normal;
            lockMovement = false;
            _verticalVelocity = -2f;

            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(jumpTimer <= 0f && !ParkourController.Instance.DetectForwardObstacle())
                {
                    state = PlayerState.Jump;
                    jumpCount++;

                    isJumping = true;
                    jumpTimer = jumpCooldown;

                    float height = jumpCount switch
                    {
                        1 => jumpHeight1,
                        2 => jumpHeight2,
                        3 => jumpHeight3,
                        _ => 1f
                    };
                        
                    _verticalVelocity = Mathf.Sqrt(height * -2f * Gravity);
                    anim.SetTrigger("jump");
                    anim.SetInteger("jumpCount", jumpCount);

                    if(jumpCount >=3 )
                        jumpCount = 0;
                }
                else
                {
                    ParkourController.Instance.TryParkour();
                }

            }


        }
        else
        {
            if(_verticalVelocity > 0f && !Input.GetKey(KeyCode.Space))
            {
                _verticalVelocity += Gravity * fallJumpMultiplier * Time.deltaTime;
                
            }
            else
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

    }

    void Move()
    {
        // handle movement
        float z = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");
        // check direction
        dir = cam.transform.right * x + cam.transform.forward * z;
        dir.y = 0;
        dir = dir.normalized;


        float targetSpeed = 0f;

        if(dir.magnitude > 0.1f)
        {
             targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, smoothRot * Time.deltaTime);

            if(!lockMovement)
                targetSpeed = Input.GetKey(KeyCode.LeftShift) ? maxSpd : normalSpd;
        }


        // calculate accleration
        float deltaSpd = targetSpeed - currSpd;
        float step = accleration * Time.deltaTime;
        deltaSpd = Mathf.Clamp(deltaSpd, -step, step);
        currSpd += deltaSpd;



        Vector3 move = dir * currSpd;
        move.y = 0;

        cc.Move(move * Time.deltaTime);
        velocity.x = Mathf.MoveTowards(velocity.x, 0, 5f * Time.deltaTime);
        velocity.z = Mathf.MoveTowards(velocity.z, 0, 5f * Time.deltaTime);
        velocity.y = _verticalVelocity;
        cc.Move(velocity * Time.deltaTime);
    }


    public void SetHasControl(bool hasControl)
    {
        this.hasControl = hasControl;
        cc.enabled = hasControl;

        if(!hasControl)
        {
            anim.SetFloat("speed", 0f);
            currSpd = 0f;
            targetRot = transform.rotation;
            
        }
    }

    public void AddForce(Vector3 force)
    {
        velocity += force;
    }

    public void SwitchVCam(VCamType type)
    {
        // Reset semua priority ke 0 terlebih dahulu agar kode lebih bersih
        VCam_3Person.Priority = 0;
        VCam_SideView.Priority = 0;
        VCam_FPS.Priority = 0;
        VCam_Sliding.Priority = 0;

        SetSideViewRotation(new Vector3(15, -90, 0));
        // SetSideViewPosition(new Vector3(10, 4, 8.5f));

        switch(type)
        {
            case VCamType.VCAM_3PERSON:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                lockInput = false;
                VCam_3Person.Priority = 10;
                break;

            case VCamType.VCAM_SIDEVIEW:
                lockInput = true;
                transform.rotation = Quaternion.Euler(0, 0, 0); 
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true; // Diperbaiki dari 'true;;'
                currSpd = 0f;
                anim.SetFloat("speed", 0);
                VCam_SideView.Priority = 10;
                
                break;

            case VCamType.VCAM_FPS:
                lockInput = true;
                transform.rotation = Quaternion.Euler(0, 0, 0); 
                currSpd = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                anim.SetFloat("speed", 0);
                VCam_FPS.Priority = 10;
                break;

            case VCamType.VCAM_SLIDING:
                anim.SetFloat("speed", 0);
                VCam_Sliding.Priority = 10;
                break;
        }
    }

    public void SetSideViewRotation(Vector3 eulerAngle)
    {
        VCam_SideView.transform.localEulerAngles = eulerAngle;
    }
    
    public void SetSideViewPosition(Vector3 pos)
    {
        VCam_SideView.transform.localPosition = pos;
    }



    public void EndJump()
    {
        isJumping = false;
    }

     public void RegisterUI()
    {
        _activeUICount++;
        UpdateCursor();
    }

    public void UnregisterUI()
    {
        _activeUICount = Mathf.Max(0, _activeUICount - 1);
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        bool anyUIActive = _activeUICount > 0;
        Cursor.lockState = anyUIActive ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = anyUIActive;
    }


    void OnDrawGizmos()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
    }


}
