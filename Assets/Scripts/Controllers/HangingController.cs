using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingController : MonoBehaviour
{
    public int rayAmount = 10;
    public float rayHeight = 1.7f;
    public float rayOffset = 0.15f;
    public float rayLength = 0.5f;
    public Player player;
    public bool inAction;

    // Start is called before the first frame update
    void Start()
    {
        player = Player.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            player.anim.SetTrigger("c");
        }
        Check();
        MatchTarget();
    }

    RaycastHit rayLedgeHit;
    RaycastHit rayDownHit;
    public LayerMask ledgeLayer;
    void Check()
    {
        if(inAction) return;

        for (int i = 0; i < rayAmount; i++)
        {
            Vector3 origin = transform.position + Vector3.up * (rayHeight + rayOffset * i);
            Debug.DrawRay(origin, transform.forward * rayLength, Color.cyan);

           if (Physics.Raycast(origin, transform.forward, out rayLedgeHit, rayLength, ledgeLayer, QueryTriggerInteraction.Ignore))
            {
                Vector3 downOrigin = rayLedgeHit.point + Vector3.up * 0.5f;
                Debug.DrawRay(downOrigin, Vector3.down * 0.7f, Color.red);

                // if (Physics.Raycast(downOrigin, Vector3.down, out rayDownHit, 0.7f, ledgeLayer))
                // {
                //     StartCoroutine(DoSwing());
                // }
                if (Physics.Raycast(downOrigin, Vector3.down, out rayDownHit, 0.7f, ledgeLayer) )
                {
                    StartCoroutine(DoSwing());
                }
            }
        }
    }

    IEnumerator DoSwing()
    {
        inAction = true;
        player.anim.applyRootMotion = true;
        transform.rotation = Quaternion.LookRotation(-rayLedgeHit.normal);
        player.SetHasControl(false);
        player.anim.CrossFade("Run And Swing", 0.2f);
        var animState = player.anim.GetCurrentAnimatorStateInfo(0);

        yield return new WaitForSeconds(animState.length);
        
        player.SetHasControl(true);
        inAction = false;
        
        yield return null;
        player.anim.applyRootMotion = false;
        yield return null;
    }


    void MatchTarget()
    {
        var stateInfo = Player.Instance.anim.GetCurrentAnimatorStateInfo(0);
        bool inTransition = Player.Instance.anim.IsInTransition(0);


        if (stateInfo.IsName("Run And Swing") && !inTransition)
        {
            Player.Instance.anim.MatchTarget(rayDownHit.point, transform.rotation, AvatarTarget.LeftHand,
            new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), 0.1f, 0.31f);
        }

    }
}
