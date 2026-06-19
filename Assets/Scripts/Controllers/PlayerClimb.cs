using System.Collections;
using UnityEngine;

public enum PlayerState2 { Normal, Climb }

public class PlayerClimb : MonoBehaviour
{
    // ─── State ────────────────────────────────────────────────────────────────
    public bool isClimbing;
    public PlayerState2 PlayerState2 = PlayerState2.Normal;

    // ─── References ───────────────────────────────────────────────────────────
    public RoofLedgeDetection roofDetection;
    public ShimmyController shimmyController;
    public Animator anim;
    public LayerMask ledgeLayer;
    public GameObject climbPointPrefab;
    public GameObject climObj;

    // ─── Ledge Detection Rays ─────────────────────────────────────────────────
    [Header("Ledge Detection")]
    public int rayAmount = 10;
    public float rayLength = 0.5f;
    public float rayOffset = 0.15f;
    public float rayHeight = 1.7f;
    public float rayHeightRoofDetect = 1.9f;

    // ─── Hop Rays ─────────────────────────────────────────────────────────────
    [Header("Hop Detection")]
    public int rayHopAmount = 10;
    public float rayHopHeight = 1.6f;
    public float rayVerticalGap = 0.8f;
    public float rayHopOffset = 0.1f;

    // ─── Match Target Corrections ─────────────────────────────────────────────
    [Header("Match Target: Grab")]
    public float rayZHandCorrection;
    public float rayYHandCorrection;

    [Header("Match Target: Drop")]
    public float rayDropZHandCorrection;
    public float rayDropYHandCorrection;

    [Header("Match Target: Hop")]
    public float HopYCorrection;
    public float HopZCorrection;

    // ─── Private ──────────────────────────────────────────────────────────────
    RaycastHit rayLedgeHit;
    RaycastHit rayDownHit;
    RaycastHit rayHopForwardHit;
    RaycastHit rayHopDownHit;
    RaycastHit rayTopRoof;

    float verticalInput;

    // ─────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        anim = GetComponent<Animator>();
        roofDetection = GetComponent<RoofLedgeDetection>();
        shimmyController = GetComponent<ShimmyController>();
    }

    void Update()
    {
        HandleClimbInput();
        HandleState();
        MatchTargetLedge();
    }

    // ─── Input Handling ───────────────────────────────────────────────────────

    RaycastHit hit;
    void HandleClimbInput()
    {
        if (!isClimbing && Player.Instance.Grounded)
        {
            roofDetection.Detect();

            if (ClimbCheck() && Input.GetKeyDown(KeyCode.C) && !roofDetection.isDetected)
                StartCoroutine(GrabLedge());
        }
        else if (isClimbing)
        {
            // check if can to roof top
            Debug.DrawRay(transform.position + new Vector3(0,rayHeightRoofDetect ,0), transform.forward, Color.blue);
            if(!Physics.Raycast(transform.position + new Vector3(0, rayHeightRoofDetect, 0), transform.forward,out hit, 1))
            {
                Debug.DrawRay(shimmyController.rayHit.point + new Vector3(0,1f ,0), Vector3.down, Color.red);
                if(Physics.Raycast(shimmyController.rayHit.point + new Vector3(0,1f ,0), Vector3.down,out rayTopRoof, 1))
                {
                    if (Input.GetKeyDown(KeyCode.X) && verticalInput == 0)
                    {
                        climObj = Instantiate(climbPointPrefab, rayTopRoof.point, Quaternion.identity);
                        
                        StartCoroutine(UpToRoof());
                        return;
                    }
                }
            }


            if (Input.GetKeyDown(KeyCode.C) && verticalInput == 0)
                StartCoroutine(DropLedge());
            else
                HandleHop();
        }
    }

    void HandleState()
    {
        bool isNormal = PlayerState2 == PlayerState2.Normal;
        anim.applyRootMotion = !isNormal;
        Player.Instance.SetHasControl(isNormal);
    }

    // ─── Ledge Detection ──────────────────────────────────────────────────────

    bool ClimbCheck()
    {
        for (int i = 0; i < rayAmount; i++)
        {
            Vector3 origin = transform.position + Vector3.up * (rayHeight + rayOffset * i);
            Debug.DrawRay(origin, transform.forward * rayLength, Color.cyan);

            if (Physics.Raycast(origin, transform.forward, out rayLedgeHit, rayLength, ledgeLayer, QueryTriggerInteraction.Ignore))
            {
                Vector3 downOrigin = rayLedgeHit.point + Vector3.up * 0.5f;
                Debug.DrawRay(downOrigin, Vector3.down * 0.7f, Color.red);

                if (Physics.Raycast(downOrigin, Vector3.down, out rayDownHit, 0.7f, ledgeLayer))
                    return true;
            }
        }

        return false;
    }

    // ─── Hop ──────────────────────────────────────────────────────────────────

    void HandleHop()
    {
        verticalInput = Input.GetAxis("Vertical");

        if (verticalInput < -0.1f)
            HopDownRayCheck();
        else if (verticalInput > 0.1f)
            HopUpRayCheck();
    }

    void HopDownRayCheck()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 origin = transform.position + Vector3.up * (rayHopHeight - rayVerticalGap - rayHopOffset * i);
            Debug.DrawRay(origin, transform.forward, Color.green);

            if (Physics.Raycast(origin, transform.forward, out rayHopForwardHit, rayLength, ledgeLayer))
            {
                Debug.Log(rayHopForwardHit.transform.gameObject);
                Vector3 downOrigin = rayHopForwardHit.point + Vector3.up * 1.35f;
                if (Physics.Raycast(downOrigin, Vector3.down, out rayHopDownHit, rayLength, ledgeLayer))
                {
                    Debug.DrawRay(downOrigin, Vector3.down, Color.red);
                    if (Input.GetKeyDown(KeyCode.C))
                        StartCoroutine(HopDown());
                }
                break;
            }
        }
    }

    void HopUpRayCheck()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 origin = transform.position + Vector3.up * (rayHopHeight + rayVerticalGap + rayHopOffset * i);
            Debug.DrawRay(origin, transform.forward, Color.green);

            if (Physics.Raycast(origin, transform.forward, out rayHopForwardHit, rayLength, ledgeLayer))
            {
                Vector3 downOrigin = rayHopForwardHit.point + Vector3.up * 0.35f;
                if (Physics.Raycast(downOrigin, Vector3.down, out rayHopDownHit, rayLength, ledgeLayer))
                {
                    Debug.DrawRay(downOrigin, Vector3.down, Color.red);
                    if (Input.GetKeyDown(KeyCode.C))
                        StartCoroutine(HopUp());
                }
                break;
            }
        }
    }

    // ─── Match Target ─────────────────────────────────────────────────────────

    void MatchTargetLedge()
    {
        var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        bool inTransition = anim.IsInTransition(0);

        if (stateInfo.IsName("Idle To Braced Hang") && !inTransition)
        {
            Vector3 handPos = rayDownHit.point
                + transform.up * rayYHandCorrection
                + transform.forward * rayZHandCorrection;

            anim.MatchTarget(handPos, transform.rotation, AvatarTarget.LeftHand,
                new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), 0.37f, 0.53f);
        }

        if (stateInfo.IsName("Drop To Freehang") && !inTransition)
        {
            Vector3 handPos = roofDetection.rayLedgeHit.point
                + transform.up * rayDropYHandCorrection
                + transform.forward * rayDropZHandCorrection;

            anim.MatchTarget(handPos, transform.rotation, AvatarTarget.LeftHand,
                new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), 0.59f, 0.71f);
        }

        if (stateInfo.IsName("Braced Hang Hop Up") && !inTransition)
        {
            Vector3 handPos = rayHopDownHit.point
                + transform.up * HopYCorrection
                + transform.forward * HopZCorrection;

            anim.MatchTarget(handPos, transform.rotation, AvatarTarget.LeftHand,
                new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), 0.39f, 0.59f);
        }

        if (stateInfo.IsName("Braced Hang To Crouch") && !inTransition)
        {
            anim.MatchTarget(climObj.transform.position, transform.rotation, AvatarTarget.LeftFoot,
                new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), 0.28f, 0.52f);
        }
    }

    // ─── Coroutines ───────────────────────────────────────────────────────────

    IEnumerator GrabLedge()
    {
        transform.rotation = Quaternion.LookRotation(-rayLedgeHit.normal);
        PlayerState2 = PlayerState2.Climb;
        anim.CrossFade("Idle To Braced Hang", 0.2f);
        yield return new WaitForSeconds(1f);
        isClimbing = true;
    }

    IEnumerator DropLedge()
    {
        anim.CrossFade("Braced Hang Drop", 0.2f);
        yield return new WaitForSeconds(0.5f);

        PlayerState2 = PlayerState2.Normal;
        isClimbing = false;

        
    }

    IEnumerator HopUp()
    {
        anim.CrossFade("Braced Hang Hop Up", 0.2f);
        yield return null;
    }

    IEnumerator HopDown()
    {
        anim.CrossFade("Braced Hang Hop Up", 0.2f);
        yield return null;
    }

    IEnumerator UpToRoof()
    {
        anim.CrossFade("Braced Hang To Crouch", 0.2f);
        yield return new WaitForSeconds(1f);
        climObj = null;
        PlayerState2 = PlayerState2.Normal;
        isClimbing = false;


        
    }

    // ─── Gizmos ───────────────────────────────────────────────────────────────

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(rayDownHit.point, 0.05f);
        Gizmos.DrawSphere(rayHopDownHit.point, 0.05f);
    }
}