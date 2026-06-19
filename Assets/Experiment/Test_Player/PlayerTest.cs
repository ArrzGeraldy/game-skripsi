using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerTest : MonoBehaviour
{
    private CharacterController cc;
    private Animator anim;

    [Header("Grounded")]
    public bool Grounded = false;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;
    [SerializeField] float Gravity = -9.81f;

    // none
    float _verticalVelocity = 0f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();    
        anim = GetComponent<Animator>();    
    }

    void Update()
    {
        GroundedCheck();
        JumpAndGravity();
        Move();
    }

    void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);

        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers);

    }

    void JumpAndGravity()
    {
        if(Grounded)
        {
            if(_verticalVelocity <= 0f)
            {
                _verticalVelocity = -2f;
            }
        }
        else
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }

    }

    void Move()
    {
        cc.Move(new Vector3(0, _verticalVelocity * Time.deltaTime, 0));
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
