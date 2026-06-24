using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public struct HitDataObstacle
{
    public RaycastHit rayForwardHit;
    public RaycastHit rayUpHit;
}

public class ParkourController : MonoBehaviour
{
    public Vector3 rayOffset = new Vector3(0, 0.25f, 0);
    public float rayForwardLength = 0.8f;
    public float rayForwardCheckLength = 4.0f;
    public float rayHeightLength = 1f;

    public static ParkourController Instance;
    public bool inAction;
    [SerializeField] List<ParkourAction> parkourActions;
    public LayerMask layer;

    public GameObject hintInput;
    Player player;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        hintInput.SetActive(false);
        player = Player.Instance;
    }

    void Update()
    {
        DebugDetectObstacle();
        MatchTarget();
    }

    void DebugDetectObstacle()
    {

        Vector3 rayPos = transform.position + rayOffset;
        Debug.DrawRay(rayPos, transform.forward * rayForwardLength, Color.white);
        bool isDetected =  Physics.Raycast(rayPos, transform.forward, rayForwardCheckLength, layer);
        bool isNormalState = Player.Instance.state == PlayerState.Normal;

        hintInput.SetActive(isDetected && isNormalState);

     
    }

    public bool TryParkour()
    {
        if(inAction) return false;
        HitDataObstacle hitData;
        Vector3 rayPos = transform.position + rayOffset;
        Debug.DrawRay(rayPos, transform.forward * rayForwardLength, Color.white);
        if(Physics.Raycast(rayPos, transform.forward, out hitData.rayForwardHit,rayForwardLength, layer))
        {
            Debug.DrawRay(hitData.rayForwardHit.point + Vector3.up * 5f, Vector3.down * 5f, Color.red);

            if(Physics.Raycast(hitData.rayForwardHit.point + Vector3.up * 5f, Vector3.down, out hitData.rayUpHit,5f, layer))
            {
                bool isNormalState = Player.Instance.state == PlayerState.Normal;
                foreach (var action in parkourActions)
                {
                    bool canParkour = action.CheckIfPossible(hitData, transform) && isNormalState;
                    Debug.Log($"can parkour: {canParkour}");

                    if (canParkour)
                    {
                        Debug.Log("possible: " + action.AnimName);
                        StartCoroutine(DoParkour(action));
                        return true; 
                    }
                }
                    
            }

        }

        return false;
    }

    public bool DetectForwardObstacle()
    {

        Vector3 rayPos = transform.position + rayOffset;
        return Physics.Raycast(rayPos, transform.forward, rayForwardCheckLength, layer);

    }

    IEnumerator DoParkour(ParkourAction action)
    {
        inAction = true;
        player.anim.applyRootMotion = true;
        player.SetHasControl(false);
        player.anim.CrossFade(action.AnimName, 0.2f);
        var animState = player.anim.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSeconds(animState.length);

        yield return new WaitForSeconds(0.9f);
        // yield return new WaitForSeconds(0.5f);
        Debug.Log("End");

        player.anim.applyRootMotion = true;
        player.SetHasControl(true);
        inAction = false;

        yield return null;
    }

    void MatchTarget()
    {
        var stateInfo = player.anim.GetCurrentAnimatorStateInfo(0);
        bool inTransition = player.anim.IsInTransition(0);

        foreach(var action in parkourActions)
        {
            if(!action.EnableTargetMatching) continue;

            if (stateInfo.IsName(action.AnimName) && !inTransition)
            {

                Player.Instance.anim.MatchTarget(action.matchPos, transform.rotation, action.MatchBodyPart,
                    new MatchTargetWeightMask(action.Weight, 0), action.MatchStartTime, action.MatchTargetTime);
            }

     
            
        }

    }

 
}
