using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Parkour System/New Parkour Action")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;

    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;

    public bool RotateToObstacle;
    public string requiredTag;
    public Quaternion TargetRotation {get;set;}
// 
    public string AnimName => animName;

    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] AvatarTarget matchBodyPart;
    [SerializeField] float matchStartTime;
    [SerializeField] float matchTargetTime;
    [SerializeField] Vector3 weight = new Vector3(0, 1, 0);

    public Vector3 matchPos {get; set;}


    public bool CheckIfPossible(HitDataObstacle hitData, Transform player)
    {
        float height = hitData.rayUpHit.point.y - player.position.y;
        if(height < minHeight || height > maxHeight)
        {
            Debug.Log($"{animName} parkour not trigger because height");
            return false;
        }


        if(!string.IsNullOrEmpty(requiredTag))
        {
            if (!hitData.rayForwardHit.collider.CompareTag(requiredTag)) return false;

            Debug.DrawRay(hitData.rayUpHit.point, Vector3.up * 0.85f, Color.red);
            if(Physics.Raycast(hitData.rayUpHit.point, Vector3.up, 0.85f))
            {
                Debug.Log("ATAS TERHALANG; TIDAK BISA MELAKUKAN " + requiredTag);
                return false;
                
            }
        }

        if(RotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.rayForwardHit.normal);

        if(enableTargetMatching)
            matchPos = hitData.rayUpHit.point;


        return true;    
    }


    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;
    public Vector3 Weight => weight;

}
